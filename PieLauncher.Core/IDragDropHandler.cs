using System.Collections;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Core;

public interface IDragDropHandler
{
    void StartDrag(IDragDropItem item);

    bool CanDrop(IDragDropItem item, IEnumerable? destination);

    void Drop(IDragDropItem item, IEnumerable? destination, int index);

    IList DropDestination{ get; }
}