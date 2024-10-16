using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Dental.WinUI
{
    partial class frmLoadSTL
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoad = new Button();
            lblFileName = new Label();
            lblErrorMessage = new Label();
            SuspendLayout();
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(175, 136);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(94, 29);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(52, 100);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(50, 20);
            lblFileName.TabIndex = 1;
            lblFileName.Text = "label1";
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.Location = new Point(52, 145);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(50, 20);
            lblErrorMessage.TabIndex = 2;
            lblErrorMessage.Text = "label2";
            // 
            // frmLoadSTL
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblErrorMessage);
            Controls.Add(lblFileName);
            Controls.Add(btnLoad);
            Name = "frmLoadSTL";
            Text = "Load STL";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        OpenFileDialog ofg = new OpenFileDialog();
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                ofg.Filter = "STereoLithography (*.stl) | *.stl";
                ofg.ShowDialog();
                if (ofg.FileNames.Length > 0)
                {
                    lblFileName.Text = ofg.FileNames[0];
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
            var windowOptions = WindowOptions.Default;
            windowOptions.API = GraphicsAPI.None;
            windowOptions.FramesPerSecond = 240;
            windowOptions.UpdatesPerSecond = 60;
            windowOptions.VSync = false;
            windowOptions.Size = new Vector2D<int>(1200, 800);

            var serviceProvider = BuildServiceProvider();
            var app = serviceProvider.GetRequiredService<App>();

            //Window.PrioritizeSdl();

            using var window = Window.Create(windowOptions);
            window.Load += async () => await app.Initialize(window, ofg.FileNames);
            window.Resize += s => app.Resize(s);
            window.Update += t => app.Update(window, t);
            window.Render += t => app.Draw(window, t);
            window.Run();
        }

        #endregion
        static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
                builder.AddSimpleConsole(options =>
                {
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                    options.TimestampFormat = "[hh:mm:ss.FFF] ";
                    options.SingleLine = true;
                }));

            services
                .AddSingleton<App>()
                .AddSingleton<IGraphicsService, GraphicsService>()
                .AddTransient<DesktopDuplicationComponent>()
                .AddTransient<TriangleComponent>()
                .AddTransient<RenderTargetComponent>()
                .AddTransient<GridComponent>()
                .AddTransient<StlMeshComponent>();

            return services.BuildServiceProvider();
        }
        private Button btnLoad;
        private Label lblFileName;
        private Label lblErrorMessage;
    }
}
