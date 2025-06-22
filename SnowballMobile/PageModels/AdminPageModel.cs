using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
using System.Collections.ObjectModel;

namespace SnowballMobile.PageModels;

public partial class AdminPanelPageModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<SnowballDto> snowballs = new();

    [ObservableProperty]
    private SnowballDto formData = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int? editingId;

    [ObservableProperty]
    private string? message;

    [ObservableProperty]
    private string? selectedImageFileName;

    private Stream? _selectedImageStream;

    public AdminPanelPageModel(ApiService apiService)
    {
        _apiService = apiService;
        LoadSnowballsCommand = new AsyncRelayCommand(LoadSnowballsAsync);
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
        EditCommand = new RelayCommand<SnowballDto>(Edit);
        DeleteCommand = new AsyncRelayCommand<int>(DeleteAsync);
        InitializeDataCommand = new AsyncRelayCommand(InitializeDataAsync);
        PickImageCommand = new AsyncRelayCommand(PickImageAsync);
    }

    public IAsyncRelayCommand LoadSnowballsCommand { get; }
    public IAsyncRelayCommand SubmitCommand { get; }
    public IRelayCommand<SnowballDto> EditCommand { get; }
    public IAsyncRelayCommand<int> DeleteCommand { get; }
    public IAsyncRelayCommand InitializeDataCommand { get; }
    public IAsyncRelayCommand PickImageCommand { get; }

    private async Task LoadSnowballsAsync()
    {
        IsBusy = true;
        try
        {
            var items = await _apiService.GetAllSnowballsAsync();
            Snowballs = new ObservableCollection<SnowballDto>(items);
        }
        catch
        {
            Message = "Failed to fetch snowballs";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Edit(SnowballDto snowball)
    {
        FormData = new SnowballDto
        {
            Name = snowball.Name,
            Price = snowball.Price,
            Description = snowball.Description,
            Image = snowball.Image,
            SnowballId = snowball.SnowballId
        };
        EditingId = snowball.SnowballId;
        SelectedImageFileName = null;
        _selectedImageStream = null;
    }

    private async Task SubmitAsync()
    {
        IsBusy = true;
        try
        {
            if (EditingId.HasValue)
            {
                await _apiService.UpdateSnowballAsync(EditingId.Value, FormData);
                Message = "Snowball updated successfully";
            }
            else
            {
                await _apiService.CreateSnowballAsync(FormData, _selectedImageStream, SelectedImageFileName);
                Message = "Snowball created successfully";
            }
            await LoadSnowballsAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            Message = $"Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync(int id)
    {
        IsBusy = true;
        try
        {
            await _apiService.DeleteSnowballAsync(id);
            Message = "Snowball deleted successfully";
            await LoadSnowballsAsync();
        }
        catch (Exception ex)
        {
            Message = $"Failed to delete: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task InitializeDataAsync()
    {
        IsBusy = true;
        try
        {
            await _apiService.InitializeAdminAsync();
            Message = "Data initialized successfully";
            await LoadSnowballsAsync();
        }
        catch (Exception ex)
        {
            Message = $"Failed to initialize data: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ResetForm()
    {
        FormData = new SnowballDto();
        EditingId = null;
        SelectedImageFileName = null;
        _selectedImageStream = null;
    }

    private async Task PickImageAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz plik obrazu",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                _selectedImageStream = await result.OpenReadAsync();
                SelectedImageFileName = result.FileName;
            }
        }
        catch
        {
            Message = "Nie udało się wybrać pliku.";
        }
    }
}