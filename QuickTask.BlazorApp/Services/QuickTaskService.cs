using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using QuickTask.Models;

namespace QuickTask.BlazorApp.Services
{
    public class QuickTaskService
    {
        public EventHandler<EventArgs> OnCollectionChanged;

        private bool _preventEventHandler = false;
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;

        public ObservableCollection<Models.QuickTaskModel> Tasks { get; set; } = new ObservableCollection<QuickTaskModel>();

        public QuickTaskService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Tasks.CollectionChanged += TasksOnCollectionChanged;

            _hubConnection = new HubConnectionBuilder().WithUrl("https://quick-task.azurewebsites.net/api").Build();
            _hubConnection.On<QuickTaskModel>("newTask", (incomingTask) =>
            {
                Console.WriteLine(incomingTask.Name);
                _preventEventHandler = true;
                Tasks.Add(incomingTask);
                _preventEventHandler = false;

                OnCollectionChanged?.Invoke(this, new EventArgs());
            });

        }

        public async Task InitializeHubConnection()
        {
            await _hubConnection.StartAsync();
        }

        private async void TasksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_preventEventHandler) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await AddRange(e.NewItems.OfType<QuickTaskModel>());

                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Remove:
                    throw new NotImplementedException();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    //ReplaceRange(e.NewItems.OfType<QuickTaskModel>());

                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task LoadAll()
        {
            var tasks = await _httpClient.GetFromJsonAsync<List<QuickTaskModel>>("getTasks");
            _preventEventHandler = true;

            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }

            _preventEventHandler = false;

            OnCollectionChanged?.Invoke(this, new EventArgs());
        }

        public async Task AddRange(IEnumerable<Models.QuickTaskModel> taskList)
        {
            foreach (var task in taskList)
            {
                await Add(task);
            }
        }

        public async Task Add(Models.QuickTaskModel task)
        {
            await _httpClient.PostAsJsonAsync("addTask", task);
        }

        //public void ReplaceRange(IEnumerable<Models.QuickTaskModel> taskList)
        //{
        //    foreach (var task in taskList)
        //    {
        //        Replace(task);
        //    }
        //}

        //public Models.QuickTaskModel Replace(Models.QuickTaskModel task)
        //{
        //    var index = Tasks.IndexOf(task);
        //    if(index == -1) throw new ArgumentException("Item does not exist.");

        //    Tasks[index] = task;

        //    return task;
        //}
    }
}
