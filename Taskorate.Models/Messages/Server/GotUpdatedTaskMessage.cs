namespace Taskorate.Models.Messages
{
  public class GotUpdatedTaskMessage : ServerMessage
  {
    public const string MethodName = nameof(GotUpdatedTaskMessage);
    public QuickTask QuickTask { get; set; }

    public GotUpdatedTaskMessage()
    {
        
    }
    public GotUpdatedTaskMessage(QuickTask task)
    {
      QuickTask = task;
    }
  }
}