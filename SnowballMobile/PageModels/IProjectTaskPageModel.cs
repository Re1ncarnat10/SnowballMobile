using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;

namespace SnowballMobile.PageModels;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}