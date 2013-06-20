using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MegaApi;

namespace MegaDesktop.Views
{
    public class TransfersViewModelDesignData
    {
        public ObservableCollection<Foo> Transfers { get; set; }

        public TransfersViewModelDesignData()
        {
            Transfers = new ObservableCollection<Foo>
            {
                new Foo{Name = "Foo1234", Progress = 50, Status = TransferHandleStatus.Downloading},
                new Foo{Name = "Foo1234", Progress = 50, Status = TransferHandleStatus.Downloading}
            };
        }
    }

    public class Foo
    {
        public string Name { get; set; }
        public int Progress { get; set; }
        public TransferHandleStatus Status { get; set; }
        public ICommand CancelCommand { get; set; }
    }
}