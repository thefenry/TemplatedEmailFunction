namespace EmailTemplates.Views
{
    public class BaseEmailClass
    {
        #region Required Configuration Settings
        private string? _viewModelType;

        public string ViewModelType
        {
            get => string.IsNullOrWhiteSpace(_viewModelType) ? this.GetType().Name : _viewModelType;
            set => _viewModelType = value;
        }

        #endregion
    }
}
