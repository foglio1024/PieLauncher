using System.Windows;

namespace PieLauncher;

public class RH
{
    protected static T Get<T>(string res)
    {
        return (T)Application.Current.FindResource(res)!;
    }
}