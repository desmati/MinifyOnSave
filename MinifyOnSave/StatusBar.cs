using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Text.RegularExpressions;
using System.Timers;

namespace MinifyOnSave
{
    public class StatusBar
    {
        private const int DotIntervalMilliseconds = 650;
        private IVsStatusbar statusBar { get; set; }
        private short _icon { get; set; }
        private Timer _dotTimer { get; set; }
        private string _dots { get; set; }
        private uint _progressBarTotalSteps { get; set; }
        private uint _progressBarStepCount { get; set; }
        private uint statusBarCookie = 0;

        public StatusBar()
        {
            statusBar = Utilities.GetStatusBar();
            _progressBarStepCount = 0;
        }

        /// <summary>
        /// Puts the status bar into build mode
        /// </summary>
        public void BuildStart()
        {
            // If icon has not yet been set to the "building" icon, set it
            if (_icon != Icons.Build)
                SetIcon(Icons.Build);

            // If we're building the entire solution, initialize the progress bar
            InitializeProgressBar();

            // Set text and icon
            SetText("Building", _icon);

            // Start dots animation
            StartDots();

            // Freeze the status bar
            Freeze();
        }

        /// <summary>
        /// Takes the status bar out of build mode
        /// </summary>
        public void BuildFinish(bool success)
        {
            // If we're building the entire solution, the progress bar was initialized and needs to be cleared
            ClearProgressBar();

            // Stop dots animation
            StopDots();

            // Stop the icon animation
            StopIcon();

            // Unfreeze the status bar
            Unfreeze();

            // Set output text
            SetText(success ? "Build successful" : "Build failed");
        }

        /// <summary>
        /// Moves a currently progressing progress bar forward one step
        /// </summary>
        public void IncrementProgressBar()
        {
            _progressBarStepCount++;
            Unfreeze();
            //statusBar.Progress(ref statusBarCookie, 1, UpdateStatusString(), _progressBarStepCount, _progressBarTotalSteps);
            statusBar.Progress(ref statusBarCookie, 1, CurrentText, _progressBarStepCount, _progressBarTotalSteps);
            Freeze();
        }

        #region Events
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IncrementDots();
            Unfreeze();
            SetText(CurrentText.Replace(".", "") + _dots);
            Freeze();
        }
        #endregion

        #region Basic Functions
        private string CurrentText
        {
            get
            {
                string text;
                statusBar.GetText(out text);
                return text;
            }
        }
        private bool IsFrozen
        {
            get
            {
                int frozen;
                statusBar.IsFrozen(out frozen);
                return Utilities.IntToBool(frozen);
            }
        }

        private void InitializeProgressBar()
        {
            _progressBarStepCount = 0;
            _progressBarTotalSteps = (uint)(Utilities.GetNumberOfProjectsToBuild() * 2);

            Unfreeze();
            //statusBar.Progress(ref statusBarCookie, 1, CreateStatusString(), _progressBarStepCount, _progressBarTotalSteps);
            statusBar.Progress(ref statusBarCookie, 1, CurrentText, _progressBarStepCount, _progressBarTotalSteps);
            Freeze();
        }
        private void ClearProgressBar()
        {
            Unfreeze();
            statusBar.Progress(ref statusBarCookie, 0, CurrentText, _progressBarStepCount, _progressBarTotalSteps);
            Freeze();
        }
        private void Clear()
        {
            statusBar.Clear();
        }
        private void SetText(string text)
        {
            Unfreeze();
            statusBar.SetText(text);
            Freeze();
        }
        private void SetText(string text, short icon)
        {
            SetIcon(icon);
            StartIcon();
            SetText(text);
        }
        private void StartIcon()
        {
            Unfreeze();
            statusBar.Animation(1, _icon);
            Freeze();
        }
        private void StopIcon()
        {
            Unfreeze();
            statusBar.Animation(0, _icon);
            Freeze();
        }
        private void SetIcon(short icon)
        {
            _icon = icon;
        }
        private void Freeze()
        {
            if (!IsFrozen)
                statusBar.FreezeOutput(1);
        }
        private void Unfreeze()
        {
            if (IsFrozen)
                statusBar.FreezeOutput(0);
        }
        private void StartDots()
        {
            _dotTimer = new Timer(DotIntervalMilliseconds);
            _dotTimer.Elapsed += timer_Elapsed;
            _dotTimer.Start();
        }
        private void StopDots()
        {
            _dotTimer.Stop();
            _dotTimer.Dispose();
        }
        private void IncrementDots()
        {
            if (_dots == null || _dots.Length == 3)
                _dots = ".";
            else
                _dots += ".";
        }
        private string GetStatusText()
        {
            var currCount = Math.Ceiling((decimal)(_progressBarStepCount * 2));
            var totalCount = _progressBarTotalSteps / 2;
            return $"{currCount} of {totalCount}";
        }
        private string CreateStatusString()
        {
            var currText = CurrentText;
            var dotsRegex = new Regex(@"(\.)+", RegexOptions.IgnoreCase);
            var dots = dotsRegex.Match(currText).Value;
            var str = currText.Replace(".", "");
            return str + " " + GetStatusText() + dots;
        }
        private string UpdateStatusString()
        {
            var statusRegex = new Regex(@"( \((\d)+ of (\d)+\))", RegexOptions.IgnoreCase);
            var dotsRegex = new Regex(@"(\.)+", RegexOptions.IgnoreCase);
            var currText = CurrentText;
            var dots = dotsRegex.Match(currText).Value;
            var status = statusRegex.Match(currText).Value;
            if (string.IsNullOrEmpty(status))
                return CreateStatusString();
            else
                return currText.Replace(status, "").Replace(".", "") + dots;
        }
        #endregion
    }
}
