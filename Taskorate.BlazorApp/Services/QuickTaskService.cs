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
using Taskorate.Models.Messages;

namespace Taskorate.BlazorApp.Services
{
  public class QuickTaskService
  {
    public EventHandler<QuickTaskList> OnTaskListLoad;
    public EventHandler<QuickTask> OnTaskAdd;
    public EventHandler<QuickTask> OnTaskChange;
    public EventHandler<string> OnTaskDelete;

    private readonly HttpClient _httpClient;
    private readonly HubConnection _hubConnection;
    private string _taskListId;
    private List<IDisposable> _hubMessageListeners;

    // public ObservableCollection<QuickTask> Tasks { get; set; }

    public QuickTaskService(HttpClient httpClient, HubConnection hubConnection)
    {
      _httpClient = httpClient;
      _hubConnection = hubConnection;

      // Tasks = new ObservableCollection<QuickTask>();
      // Tasks.CollectionChanged += TasksOnCollectionChanged;

      _hubMessageListeners = new List<IDisposable>();
    }

    // public void ClearTasks()
    // {
    //   Tasks = new ObservableCollection<QuickTask>();
    //   Tasks.CollectionChanged += TasksOnCollectionChanged;

    //   OnCollectionChanged?.Invoke(this, new EventArgs());
    // }

    public void SetTaskListId(string id)
    {
      _taskListId = id;
    }

    public async Task InitializeHubConnection()
    {
      // Dispose listeners of previous task lists
      foreach (var listener in _hubMessageListeners)
      {
        listener?.Dispose();
      }
      _hubMessageListeners.Clear();

      AddHubMessageHandler<GotSingleTaskListMessage>(GotSingleTaskListMessage.MethodName, m => OnTaskListLoad?.Invoke(this, m.TaskList));
      AddHubMessageHandler<GotNewTaskMessage>(GotNewTaskMessage.MethodName, m => OnTaskAdd?.Invoke(this, m.QuickTask));
      AddHubMessageHandler<GotUpdatedTaskMessage>(GotUpdatedTaskMessage.MethodName, m => OnTaskChange(this, m.QuickTask));
      AddHubMessageHandler<GotDeletedTaskMessage>(GotDeletedTaskMessage.MethodName, m => OnTaskDelete(this, m.QuickTaskId));

      if (_hubConnection.State == HubConnectionState.Disconnected)
      {
        await _hubConnection.StartAsync();
      }
    }

    // private void TasksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    // {
    //   if (_preventEventHandler) return;

    //   switch (e.Action)
    //   {
    //     case NotifyCollectionChangedAction.Add:
    //       AddRange(e.NewItems.OfType<QuickTask>());

    //       break;
    //     case NotifyCollectionChangedAction.Move:
    //       throw new NotImplementedException();

    //     case NotifyCollectionChangedAction.Remove:
    //       throw new NotImplementedException();

    //     case NotifyCollectionChangedAction.Replace:
    //       UpdateRange(e.NewItems.OfType<QuickTask>());

    //       break;
    //     case NotifyCollectionChangedAction.Reset:
    //       throw new NotImplementedException();

    //     default:
    //       throw new ArgumentOutOfRangeException();
    //   }
    // }

    private void AddHubMessageHandler<T>(string methodName, Action<T> handler) where T : ServerMessage
    {
      _hubMessageListeners.Add(_hubConnection.On<T>(methodName, argument =>
      {
        // Ignore this message if it's ment to be ignored by the server...
        if(argument.IgnoreConnectionId != _hubConnection.ConnectionId) {
          handler(argument);
        }
      }));
    }

    public void LoadTasks()
    {
      _ = _hubConnection.SendAsync(GetSingleTaskListMessage.MethodName, new GetSingleTaskListMessage(_taskListId));
    }

    public void AddRange(IEnumerable<QuickTask> taskList)
    {
      foreach (var task in taskList)
      {
        Add(task);
      }
    }

    public void Add(QuickTask task)
    {
      _ = _hubConnection.SendAsync(AddTaskMessage.MethodName, new AddTaskMessage(_taskListId, task));
    }

    public void UpdateRange(IEnumerable<QuickTask> taskList)
    {
      foreach (var task in taskList)
      {
        Update(task);
      }
    }

    public void Update(QuickTask task)
    {
      _ = _hubConnection.SendAsync(UpdateTaskMessage.MethodName, new UpdateTaskMessage(_taskListId, task));
    }

    public void Delete(QuickTask task)
    {
      _ = _hubConnection.SendAsync(DeleteTaskMessage.MethodName, new DeleteTaskMessage(_taskListId, task.Id));
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
