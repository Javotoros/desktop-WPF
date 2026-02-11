using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System.Windows.Input;
using System.Windows;
using System.Text.Json;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace DesktopApp.ViewModels
{
    /// <summary>
    /// ViewModel del formulario de Habitaciones.
    /// Se encarga de:
    /// - Crear habitaciones (modo alta)
    /// - Editar habitaciones (modo edición)
    /// - Validar campos para habilitar/deshabilitar el botón Guardar
    /// - Gestionar imágenes (seleccionar, quitar, subir y borrar)
    /// </summary>
    public class FormRoomsViewModel:INotifyPropertyChanged
    {
        // Cliente para llamar a la API (GET/POST/PATCH/DELETE)
        private readonly ApiClient _api = new ApiClient();

        // Evento necesario para que WPF actualice la vista cuando cambian propiedades (binding)
        public event PropertyChangedEventHandler? PropertyChanged;

        //Tipos de variables enums con los que relleno el combobox
        public Array RoomTypes => Enum.GetValues(typeof(Rooms.RoomType));
        public Array AvailabilityOptions => Enum.GetValues(typeof(Rooms.Availability));

        // Lista que se muestra en pantalla (preview de imágenes: nombres o rutas)
        public ObservableCollection<string> Images { get; } = new();

        // Archivos locales seleccionados (se suben después a la API)
        public ObservableCollection<string> LocalImagesToUpload { get; } = new();

        // Imágenes ya existentes en servidor que el usuario quiere borrar
        public ObservableCollection<string> RemoteImagesToDelete { get; } = new();

        // Copia de las imágenes originales para saber qué había antes al editar
        private List<string> _originalRemoteImages = new();

        // Texto informativo de la cantidad de img
        private string? _imageUrlDraft;
        public string? ImageUrlDraft
        {
            get => _imageUrlDraft;
            set { _imageUrlDraft = value; OnPropertyChanged(); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        private int? _currentRoom;
        public int? CurrentRoom
        {
            get => _currentRoom;
            set { _currentRoom = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Habitación seleccionada para editar (recibida desde otra pantalla).
        /// </summary>
        private Rooms? _selectedRoom;
        public Rooms? SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Siguiente número de habitación calculado para la planta seleccionada.
        /// Lo devuelve la API para evitar duplicados.
        /// </summary>
        private int _nextRoom;
        public int NextRoom
        {
            get => _nextRoom;
            set { _nextRoom = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Planta de la habitación. Al cambiarla:
        /// - se consulta a la API el siguiente número de habitación
        /// - se recalcula CanSave (botón guardar)
        /// </summary>
        private int? _newFloor;
        public int? NewFloor
        {
            get => _newFloor;
            set
            {
                _newFloor = value;
                OnPropertyChanged();
                _ = LoadNextRoom();
                CommandManager.InvalidateRequerySuggested();
            }
        }


        private Rooms.RoomType? _roomType;
        public Rooms.RoomType? RoomType
        {
            get => _roomType;
            set
            {
                _roomType = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private int? _pricePerNight;
        public int? PricePerNight
        {
            get => _pricePerNight;
            set
            {
                _pricePerNight = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int? _maxOccupancy;
        public int? MaxOccupancy
        {
            get => _maxOccupancy;
            set
            {
                _maxOccupancy = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Rooms.Availability? _availability;
        public Rooms.Availability? Availability
        {
            get => _availability;
            set
            {
                _availability = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Comandos para botones de la interfaz
        public ICommand SaveCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PickImagesCommand { get; }
        public ICommand RemoveImageCommand { get; }

        /// <summary>
        /// Constructor para crear una habitación.
        /// </summary>
        public FormRoomsViewModel()
        {

            IsEditing = true;
            SaveCommand = new RelayCommand(async _ => await SendDataRooms(), _ => CanSave());
            CancelCommand = new RelayCommand(w => CloseWindow(w as Window));
            LimpiarCommand = new RelayCommand(_ => Clean());

            PickImagesCommand = new RelayCommand(_ => PickImages());
            RemoveImageCommand = new RelayCommand( p =>  RemoveImage(p as string));


        }

        /// <summary>
        /// Constructor para editar una habitación.
        /// Carga los datos iniciales y las imágenes desde la API.
        /// </summary>
        public FormRoomsViewModel(Rooms room):this()
        {
            SelectedRoom = room;
            IsEditing = false;
            CurrentRoom = room.numRoom;
            NewFloor = room.numFloor;
            RoomType = room.roomType;
            Description = room.description;
            PricePerNight = (int)room.pricePerNight;
            MaxOccupancy = room.maxOccupancy;
            Availability = room.availability;

            SaveCommand = new RelayCommand(async w => await UpdateDataRooms(w as Window), _ => CanSave());
            LimpiarCommand = new RelayCommand(_ => CleanUpdate());
            RemoveImageCommand = new RelayCommand(async p => await DataRemoveImage(p as string));
            PickImagesCommand = new RelayCommand(_ => PickImagesUpdate());
            // Cargamos imágenes existentes
            _ = LoadImagesFromApi();
        }

        /// <summary>
        /// Abre un diálogo para seleccionar imágenes del ordenador.
        /// Se guardan en LocalImagesToUpload para subirlas cuando se guarde.
        /// </summary>
        private void PickImages()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.webp",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    if (LocalImagesToUpload.Contains(file)) continue;

                    LocalImagesToUpload.Add(file);

                    var previewLine = $"{System.IO.Path.GetFileName(file)}";
                    Images.Add(previewLine);

                    
                }
                ImageUrlDraft = $"{Images.Count} imagen(es)";
            }
        }
        private void PickImagesUpdate()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.webp",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    if (LocalImagesToUpload.Contains(file)) continue;

                    if (LocalImagesToUpload.Contains(file)) continue;

                    LocalImagesToUpload.Add(file);

                    var previewLine = $"{System.IO.Path.GetFileName(file)}";
                    Images.Add(previewLine);
                }
                ImageUrlDraft = $"{Images.Count} imagen(es)";
            }
        }

        /// <summary>
        /// Elimina una imagen de la lista al crear.
        /// Quita la preview y el archivo local correspondiente al crear.
        /// </summary>
        private void RemoveImage(string? item)
        {
            if (string.IsNullOrWhiteSpace(item)) return;

            var index = Images.IndexOf(item);
            if (index < 0) return;

            Images.RemoveAt(index);

            if (index >= 0 && index < LocalImagesToUpload.Count) LocalImagesToUpload.RemoveAt(index);

            ImageUrlDraft = $"{LocalImagesToUpload.Count} imagen(es)";
        }

        private async Task DataRemoveImage(string? item)
        {
            if (string.IsNullOrWhiteSpace(item)) return;

            if (!Images.Contains(item)) return;
            Images.Remove(item);

            if (LocalImagesToUpload.Contains(item))
            {
                LocalImagesToUpload.Remove(item);
                ImageUrlDraft = $"{Images.Count} imagen(es)";
                return;
            }

            if (item.StartsWith("/uploads/"))
            {
                if (!RemoteImagesToDelete.Contains(item))
                    RemoteImagesToDelete.Add(item);
            }
            
            ImageUrlDraft = $"{Images.Count} imagen(es)";
        }

        private bool CanSave()
        {
            if (NewFloor.HasValue
                && NewFloor.Value >= 1 && NewFloor.Value <= 7
                && PricePerNight.HasValue && PricePerNight.Value >= 1
                && MaxOccupancy.HasValue && MaxOccupancy.Value >= 1 && MaxOccupancy.Value <= 4
                && RoomType.HasValue
                && Availability.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Clean()
        {
            NewFloor = null;
            RoomType = null;
            Description = null;
            PricePerNight = null;
            MaxOccupancy = null;
            Availability = null;

            CommandManager.InvalidateRequerySuggested();
        }
        private void CleanUpdate()
        {
            RoomType = null;
            Description = null;
            PricePerNight = null;
            MaxOccupancy = null;
            Availability = null;

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Consulta a la API el siguiente número de habitación disponible en esa planta.
        /// Si la planta es inválida, pone NextRoom = 0.
        /// </summary>
        private async Task LoadNextRoom()

        {
            if (NewFloor is null || NewFloor < 1 || NewFloor > 7)
            {
                NextRoom = 0;
                return;
            }
            try
            {
                NextRoom = await _api.GetNextRoom(NewFloor.Value);

            }
            catch (Exception e)
            {
                NextRoom = 0;
                MessageBox.Show(e.Message);
            }


        }


        private async Task SendDataRooms()
        {
            try
            {
                string body = await _api.PostRooms(NewFloor!.Value,
                    RoomType.Value.ToString(),
                    Description ?? "",
                    PricePerNight!.Value,
                    MaxOccupancy!.Value,
                    Availability.Value.ToString()
                );


                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var id = root.GetProperty("id").GetString();
                if (string.IsNullOrWhiteSpace(id))
                    throw new Exception("La API no devolvió el id de la habitación.");

                if (LocalImagesToUpload.Count > 0)
                {
                    await _api.PostIdImgRoom(id, LocalImagesToUpload.ToList());
                }

                MessageBox.Show(
                    $"{root.GetProperty("message").GetString()}\n\n" +
                    $"Número habitación: {root.GetProperty("numRoom").GetInt32()}\n" +
                    $"Planta: {root.GetProperty("numFloor").GetInt32()}",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                Clean();
                Images.Clear() ;
                LocalImagesToUpload.Clear();
                ImageUrlDraft = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private async Task UpdateDataRooms(Window? w)
        {
            try
            {
                await _api.updateIdRoom(SelectedRoom!.Id,
                    RoomType.Value.ToString(),
                    Description ?? "",
                    PricePerNight!.Value,
                    MaxOccupancy!.Value,
                    Availability.Value.ToString()
                );

                if (LocalImagesToUpload.Count > 0)
                    await _api.PostIdImgRoom(SelectedRoom!.Id, LocalImagesToUpload.ToList());

                foreach (var img in RemoteImagesToDelete.ToList())
                    await _api.DeleteIdImgRoom(SelectedRoom!.Id, img);
                MessageBox.Show(
                    "Habitación actualizada\n\n" +
                    $"Número habitación: {SelectedRoom.numRoom}\n" +
                    $"Planta: {SelectedRoom.numFloor}",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                CloseWindow(w);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        private async Task LoadImagesFromApi()
        {
            if (SelectedRoom?.Id == null) return;

            Images.Clear();
            LocalImagesToUpload.Clear();
            RemoteImagesToDelete.Clear();

            _originalRemoteImages = SelectedRoom.image?.ToList() ?? new List<string>();

            foreach (var img in _originalRemoteImages)
                Images.Add(img);

            ImageUrlDraft = $"{Images.Count} imagen(es)";

        }


        private void CloseWindow(Window? w)
        {
            w?.Close();
        }

        /// <summary>
        /// Notifica a la vista que una propiedad ha cambiado (binding WPF).
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
