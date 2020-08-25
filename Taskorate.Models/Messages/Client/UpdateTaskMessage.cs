namespace Taskorate.Models.Messages
{
  public class UpdateTaskMessage
  {
    public const string MethodName = nameof(UpdateTaskMessage);
    public string TaskListId { get; set; }
    public QuickTask QuickTask { get; set; }

    public UpdateTaskMessage()
    {
        
    }
    public UpdateTaskMessage(string taskListId, QuickTask task)
    {
      TaskListId = taskListId;
      QuickTask = task;
    }
  }
}