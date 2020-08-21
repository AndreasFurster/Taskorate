using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Taskorate.Models;

namespace Taskorate.BlazorApp.Services
{
    public class QuickTaskService
    {
        public EventHandler<EventArgs> OnCollectionChanged;

        private bool _preventEventHandler;
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;
        private Guid _taskListId;
        private IDisposable _newTaskMessageListener;
        private IDisposable _updatedTaskMessageListener;

        public ObservableCollection<QuickTask> Tasks { get; set; }

        public QuickTaskService(HttpClient httpClient, HubConnection hubConnection)
        {
            _httpClient = httpClient;
            _hubConnection = hubConnection;

            Tasks = new ObservableCollection<QuickTask>();
            Tasks.CollectionChanged += TasksOnCollectionChanged;
        }

        public void ClearTasks()
        {
            Tasks = new ObservableCollection<QuickTask>();
            Tasks.CollectionChanged += TasksOnCollectionChanged;

            OnCollectionChanged?.Invoke(this, new EventArgs());
        }

        public void SetTaskListId(Guid id)
        {
            _taskListId = id;
        }

        public async Task InitializeHubConnection()
        {
            // Dispose listeners of previous task lists
            _newTaskMessageListener?.Dispose();
            _updatedTaskMessageListener?.Dispose();

            _newTaskMessageListener = _hubConnection.On<QuickTask>($"{_taskListId}/newTask", incomingTask =>
            {
                _preventEventHandler = true;
                Tasks.Add(incomingTask);
                _preventEventHandler = false;

                OnCollectionChanged?.Invoke(this, new EventArgs());
            });

            _updatedTaskMessageListener = _hubConnection.On<QuickTask>($"{_taskListId}/updatedTask", incomingTask =>
            {
                _preventEventHandler = true;

                // Find task with same id
                var existingTask = Tasks.FirstOrDefault(t => t.Id == incomingTask.Id);

                if (existingTask != null)
                {
                    var index = Tasks.IndexOf(existingTask);
                    Tasks[index] = incomingTask;
                }

                _preventEventHandler = false;

                OnCollectionChanged?.Invoke(this, new EventArgs());
            });

            _updatedTaskMessageListener = _hubConnection.On<Guid>($"{_taskListId}/deletedTask", id =>
            {
                _preventEventHandler = true;

                // Find task with same id
                var existingTask = Tasks.FirstOrDefault(t => t.Id == id);

                if (existingTask != null)
                {
                    var index = Tasks.IndexOf(existingTask);
                    Tasks.RemoveAt(index);
                }

                _preventEventHandler = false;

                OnCollectionChanged?.Invoke(this, new EventArgs());
            });

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        private async void TasksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_preventEventHandler) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await AddRange(e.NewItems.OfType<QuickTask>());

                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Remove:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Replace:
                    await UpdateRange(e.NewItems.OfType<QuickTask>());

                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task LoadTasks()
        {
            var taskList = await _httpClient.GetFromJsonAsync<QuickTaskList>($"task-lists/{_taskListId}");
            _preventEventHandler = true;

            if (taskList?.Tasks != null)
            {
                Tasks.Clear();
                foreach (var task in taskList.Tasks)
                {
                    Tasks.Add(task);
                }
            }

            _preventEventHandler = false;

            OnCollectionChanged?.Invoke(this, new EventArgs());
        }

        public async Task AddRange(IEnumerable<QuickTask> taskList)
        {
            foreach (var task in taskList)
            {
                await Add(task);
            }
        }

        public async Task Add(QuickTask task)
        {
            await _httpClient.PostAsJsonAsync($"task-lists/{_taskListId}/tasks", task);
        }

        public async Task UpdateRange(IEnumerable<QuickTask> taskList)
        {
            foreach (var task in taskList)
            {
                await Update(task);
            }
        }

        public async Task Update(QuickTask task)
        {
            await _httpClient.PutAsJsonAsync($"task-lists/{_taskListId}/tasks", task);
        }

        public async Task Delete(QuickTask task)
        {
            await _httpClient.DeleteAsync($"task-lists/{_taskListId}/tasks/{task.Id}");
        }

        //public void ReplaceRange(IEnumerable<Models.QuickTask> taskList)
        //{
        //    foreach (var task in taskList)
        //    {
        //        Replace(task);
        //    }
        //}

        //public Models.QuickTask Replace(Models.QuickTask task)
        //{
        //    var index = Tasks.IndexOf(task);
        //    if(index == -1) throw new ArgumentException("Item does not exist.");

        //    Tasks[index] = task;

        //    return task;
        //}

    }
}
