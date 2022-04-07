using Ponder.Sln;

namespace Ponder.Display;

public sealed class SlnDisplay : IBusListener<SlnState>
{
    public Task OnPublish(SlnState message)
    {
        Console.Clear();

        foreach (var project in message.Projects)
        {
            if (project is LoadingProjectState)
            {
                Console.WriteLine($"{project.Name} ({project.Path}) ⌛");
            }
            else if (project is LoadedProjectState loadedState)
            {
                var testIndicator = loadedState.Project.IsTestProject
                    ? "|_|"
                    : "";
                Console.WriteLine(
                    $"{project.Name} ({project.Path}) {testIndicator}");
            }

        }

        return Task.CompletedTask;
    }
}
