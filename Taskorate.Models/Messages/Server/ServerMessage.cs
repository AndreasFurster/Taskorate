namespace Taskorate.Models.Messages
{
  public abstract class ServerMessage
  {
    // Connection ID who can ignore this message. 
    // TODO: Implement correct "SendToOthers" method
    public string IgnoreConnectionId { get; set; }
  }
}