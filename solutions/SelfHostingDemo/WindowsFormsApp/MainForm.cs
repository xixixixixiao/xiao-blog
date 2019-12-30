using System;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using WindowsFormsApp.Services;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Http service.
        /// </summary>
        private readonly HttpService _http;

        public MainForm()
        {
            InitializeComponent();

            /**
             * enable the start button and disable the close button.
             */
            this.StartButton.Enabled = true;
            this.CloseButton.Enabled = false;

            /**
             * initialize http service.
             */
            _http = new HttpService();
        }

        /// <summary>
        /// start the http server.
        /// </summary>
        private void StartButton_Click(object sender, EventArgs e)
        {
            /**
             * start.
             */
            try
            {
                var port = this.PortNum.Value;

                _http.StartHttpServer($"{port}");

                MessageBox.Show("Start successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                /**
                 * disable the start button and enable the close button.
                 */
                this.StartButton.Enabled = false;
                this.PortNum.Enabled     = false;
                this.CloseButton.Enabled = true;
            }
            catch (AggregateException exception)
            {
                if (exception.Flatten().InnerExceptions.Any(ex => ex is AddressAccessDeniedException))
                {
                    MessageBox.Show("Permission denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// close the http server.
        /// </summary>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            /**
             * close.
             */
            try
            {
                _http.CloseHttpServer();

                MessageBox.Show("Close successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                /**
                 * disable the start button and enable the close button.
                 */
                this.StartButton.Enabled = true;
                this.PortNum.Enabled     = true;
                this.CloseButton.Enabled = false;

            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
