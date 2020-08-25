namespace Taskorate.Models.Messages
{
  public class GotDeletedTaskMessage : ServerMessage
  {
    public const string MethodName = nameof(GotDeletedTaskMessage);
    public string QuickTaskId { get; set; }

    public GotDeletedTaskMessage()
    {
        
    }
    public GotDeletedTaskMessage(string taskId)
    {
      QuickTaskId = taskId;
    }
  }
}