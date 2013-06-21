using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaDesktop.Util;

namespace MegaDesktop.ViewModels
{
    internal class ToolBarViewModel : INotifyPropertyChanged
    {
        private NodeViewModel _selectedNode;

        public ToolBarViewModel(NodeManager nodeManager, IEnumerable<IToolBarCommand> toolBarCommands)
        {
            nodeManager.SelectedNodeChanged += (s, e) => SelectedNode = nodeManager.SelectedNode;

            Commands = toolBarCommands.OrderBy(x => x.Position);
        }

        public IEnumerable<IToolBarCommand> Commands { get; private set; }

        public NodeViewModel SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                OnPropertyChanged();

                Commands.ForEach(x => x.OnCanExecuteChanged());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}