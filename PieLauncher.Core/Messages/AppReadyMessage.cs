namespace PieLauncher.Core.Messages;

public record AppReadyMessage
{
    public static AppReadyMessage Instance { get; } = new();
}