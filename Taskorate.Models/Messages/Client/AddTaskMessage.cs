namespace Taskorate.Models.Messages
{
  public class AddTaskMessage
  {
    public const string MethodName = nameof(AddTaskMessage);
    public string TaskListId { get; set; }
    public QuickTask QuickTask { get; set; }

    public AddTaskMessage()
    {
        
    }
    public AddTaskMessage(string taskListId, QuickTask task)
    {
      TaskListId = taskListId;
      QuickTask = task;
    }
  }
}