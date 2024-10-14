using PieLauncher.Core.ViewModels;

namespace PieLauncher.Core.Messages;

public record ShortcutHoveredMessage(ShortcutViewModel Sender, bool IsHovered);