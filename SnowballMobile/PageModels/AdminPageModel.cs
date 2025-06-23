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
        TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
    }

    public IAsyncRelayCommand LoadSnowballsCommand { get; }
    public IAsyncRelayCommand SubmitCommand { get; }
    public IRelayCommand<SnowballDto> EditCommand { get; }
    public IAsyncRelayCommand<int> DeleteCommand { get; }
    public IAsyncRelayCommand InitializeDataCommand { get; }
    public IAsyncRelayCommand PickImageCommand { get; }
    public IAsyncRelayCommand TakePhotoCommand { get; }
    public Stream? SelectedImageStream { get; set; }

    private async Task LoadSnowballsAsync()
    {
        IsBusy = true;
        try
        {
            var items = await _apiService.GetAllSnowballsAsync();
            Snowballs.Clear();
            foreach (var item in items)
                Snowballs.Add(item);
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
            if (string.IsNullOrWhiteSpace(FormData.Name) || string.IsNullOrWhiteSpace(FormData.Description))
            {
                Message = "Uzupełnij wszystkie pola.";
                return;
            }

            bool result;
            if (EditingId.HasValue)
            {
                if (_selectedImageStream == null || string.IsNullOrEmpty(SelectedImageFileName))
                {
                    Message = "Wybierz zdjęcie produktu.";
                    return;
                }
                result = await _apiService.UpdateSnowballAsync(
                    EditingId.Value,
                    FormData,
                    _selectedImageStream,
                    SelectedImageFileName
                );
                Message = result ? "Snowball updated successfully" : "Update failed";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FormData.Image))
                    FormData.Image = "snowball.png";

                result = await _apiService.CreateSnowballAsync(FormData, _selectedImageStream, SelectedImageFileName);
                Message = result ? "Snowball created successfully" : "Creation failed";
            }

            if (result)
            {
                await LoadSnowballsAsync();
                ResetForm();
            }
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
        _selectedImageStream?.Dispose();
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
                FormData.Image = result.FileName;
            }
        }
        catch
        {
            Message = "Nie udało się wybrać pliku.";
        }
    }

    private async Task TakePhotoAsync()
    {
        try
        {
            var result = await MediaPicker.Default.CapturePhotoAsync();
            if (result != null)
            {
                SelectedImageFileName = result.FileName;
                SelectedImageStream = await result.OpenReadAsync();
            }
        }
        catch (Exception ex)
        {
            Message = $"Błąd podczas robienia zdjęcia: {ex.Message}";
        }
    }
}