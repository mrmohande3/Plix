using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PlixP.Repositories.Contracts;
using PlixP.Services.Contracts;

namespace PlixP.ViewModels
{
    public class GenreItemModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Image { get; set; }
        public int MovieCount { get; set; }
    }
    public class GenresDialogViewModel : BindableBase
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        public event EventHandler<string> GenreSelected;
        public ICommand GenreSelectCommand { get; set; }
        public ObservableCollection<GenreItemModel> Genres { get; set; } = new ObservableCollection<GenreItemModel>();
        public GenresDialogViewModel(IServiceWrapper serviceWrapper,IRepositoryWrapper repositoryWrapper)
        {
            _serviceWrapper = serviceWrapper;
            _repositoryWrapper = repositoryWrapper;
            GenreSelectCommand = new DelegateCommand<GenreItemModel>(model =>
            {
                GenreSelected?.Invoke(model, model.Name);
            });
            Initialize();
        }

        private async Task Initialize()
        {

            (await _serviceWrapper.MovieServices.GetGenresModel()).ForEach(g=>Genres.Add(g));
        }
    }
}
