namespace PieLauncher.Core.Messages;

public record AppExitingMessage
{
    public static AppExitingMessage Instance { get; } = new();
}