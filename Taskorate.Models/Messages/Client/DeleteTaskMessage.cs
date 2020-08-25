namespace Taskorate.Models.Messages
{
  public class DeleteTaskMessage
  {
    public const string MethodName = nameof(DeleteTaskMessage);
    public string TaskListId { get; set; }
    public string QuickTaskId { get; set; }

    public DeleteTaskMessage()
    {
        
    }
    public DeleteTaskMessage(string taskListId, string taskId)
    {
      TaskListId = taskListId;
      QuickTaskId = taskId;
    }
  }
}