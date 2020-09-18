using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using ShellDuo.Models;
using ShellDuo.Views;
using Xamarin.Forms.DualScreen;
using System.ComponentModel;

namespace ShellDuo.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public new void OnAppearing(bool isDetail = false)
        {
            base.OnAppearing(isDetail);
            if (!IsDetail) SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            if (!DeviceIsSpanned && !DeviceIsBigScreen && string.IsNullOrEmpty(ItemId))
            {
                await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemId)}={item.Id}");
            }
        }

        private string itemId;

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            IsBusy = true;

            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                SelectedItem = item;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected override void UpdateLayouts()
        {
            UpdateLayouts(SelectedItem != null, $"{nameof(ItemDetailPage)}?{nameof(ItemsViewModel.ItemId)}={SelectedItem?.Id}");
        }
    }
}