/// <summary>
/// The main namespace.
/// </summary>
namespace xboxTetris
{
    using System;
    using Windows.UI.Popups;

    /// <summary>
    /// A class that contains commands for the UWP message boxes.
    /// </summary>
    class UWP_MessageBox
    {

        #region popupMessage

        /// <summary>
        /// A Action delegate for the "Yes" button
        /// </summary>
        private Action Yes;

        /// <summary>
        /// A Action delegate for the "No" button
        /// </summary>
        private Action No;

        /// <summary>
        /// Pops up with a UI message simmalar to "MessageBox.Show()"
        /// </summary>
        public async void popupMessage(string content, string titleContent)
        {
            MessageDialog messageDialog = new MessageDialog(content, titleContent);
            await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Pops up with a UI message simmalar to "MessageBox.Show()", with a Yes & No function.
        /// </summary>
        public async void popupMessage(string content, string titleContent, Action Yes, Action No)
        {
            this.Yes = Yes;
            this.No = No;
            MessageDialog messageDialog = new MessageDialog(content, titleContent);
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.Yes_No_Click)));
            messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.Yes_No_Click)));
            await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Runs the passed Yes or No method acording to what the user sets.
        /// </summary>
        /// <param name="command"></param>
        private void Yes_No_Click(IUICommand command)
        {
            if(command.Label == "Yes")
            {
                this.Yes();
            }

            else
            {
                this.No();
            }
        }

        #endregion
    }
}
