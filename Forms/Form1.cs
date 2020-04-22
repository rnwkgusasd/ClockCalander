using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClockCalender
{
    public partial class Form1 : Form
    {
        #region Member Variables
        // =======================================================
        // Label Mouse Event Variable
        // =======================================================
        private Color PanelColor = Color.WhiteSmoke;

        // =======================================================
        // Form Drag Event Variable
        // =======================================================
        private bool IsDraging = false;

        private Point ClickPos;
        private Point CurrMousePos;

        private Thread th;

        // =======================================================
        // Current Date Time Variable
        // =======================================================
        private DateTime CurrDateTime;

        // =======================================================
        // Current Date Time Variable
        // =======================================================
        private bool IsRunningClock = true;
        private Thread Clock;
        #endregion

        #region Enum
        public enum DayOfWeek_IDX
        {
            SUNDAY = 4,
            MONDAY = 3,
            TUESDAY = 2,
            WEDNESDAY = 1,
            THURSDAY = 0,
            FRIDAY = 6,
            SATURDAY = 5
        }
        #endregion

        #region Initialize
        public Form1()
        {
            InitializeComponent();

            this.Text = "Clock && Calendar";
        }
        #endregion

        #region Label Events
        public void LabelMouseHoverIn_Event(object sender, EventArgs e)
        {
            Label tP = (Label)sender;
            PanelColor = tP.BackColor;
            tP.BackColor = Color.AliceBlue;
        }

        public void LabelMouseHoverOut_Event(object sender, EventArgs e)
        {
            Label tP = (Label)sender;
            tP.BackColor = PanelColor;
        }
        #endregion

        #region Form Drag Events
        private void DragForm(object sender, MouseEventArgs e)
        {
            IsDraging = true;

            ClickPos.Y = e.Y;
            ClickPos.X = e.X;
            CurrMousePos = Cursor.Position;

            th = new Thread(DragThread);

            th.Start();
        }

        private void DragOutForm(object sender, MouseEventArgs e)
        {
            IsDraging = false;
        }

        private void DragThread()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                while (IsDraging)
                {
                    CurrMousePos = new Point(Cursor.Position.X - ClickPos.X, Cursor.Position.Y - ClickPos.Y);

                    Invoke(new Action(() => this.Location = CurrMousePos));

                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
        }
        #endregion

        #region Button Click Events
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            CurrDateTime = CurrDateTime.AddMonths(-1);

            ChangeCalendar();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            CurrDateTime = CurrDateTime.AddMonths(1);

            ChangeCalendar();
        }

        private void ChangeCalendar()
        {
            if (CurrDateTime.Year == DateTime.Now.Year && CurrDateTime.Month == DateTime.Now.Month)
                SetCalendar(CurrDateTime.Year, CurrDateTime.Month, CurrDateTime.Day, true);
            else SetCalendar(CurrDateTime.Year, CurrDateTime.Month, CurrDateTime.Day);
        }
        #endregion

        #region Clock Thread Function
        private void TimeThread()
        {
            ManualResetEvent tMr = new ManualResetEvent(false);

            while(IsRunningClock)
            {
                if(!tMr.WaitOne(300))
                {
                    string tNowTime = DateTime.Now.ToString("hh:mm");
                    lblTime?.Invoke(new Action(() =>
                    {
                        if (lblTime.Text != tNowTime) lblTime.Text = tNowTime;
                    }));
                    Application.DoEvents();
                }
            }
        }
        #endregion

        #region Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            CurrDateTime = DateTime.Now;
            SetCalendar(CurrDateTime.Year, CurrDateTime.Month, CurrDateTime.Day, true);

            Clock = new Thread(TimeThread);
            Clock.Start();
        }
        #endregion

        #region Main Function
        public void SetCalendar(int pYear, int pMonth, int pDay, bool pIsToday=false)
        {
            ClearCalendar();

            lblMonth.Text = pMonth.ToString();
            lblYear.Text = pYear.ToString();

            DateTime tNowTime = new DateTime(pYear, pMonth, pDay);
            DateTime tFirstDay = tNowTime.AddDays(1 - pDay);
            DateTime tLastDay = tNowTime.AddMonths(1).AddDays(0 - pDay);

            DayOfWeek_IDX tFirstDay_DayofWeek = 0;

            switch(tFirstDay.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.SUNDAY;
                    break;

                case DayOfWeek.Monday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.MONDAY;
                    break;

                case DayOfWeek.Tuesday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.TUESDAY;
                    break;

                case DayOfWeek.Wednesday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.WEDNESDAY;
                    break;

                case DayOfWeek.Thursday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.THURSDAY;
                    break;

                case DayOfWeek.Friday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.FRIDAY;
                    break;

                case DayOfWeek.Saturday:
                    tFirstDay_DayofWeek = DayOfWeek_IDX.SATURDAY;
                    break;
            }

            int tWeekCnt = 0;

            for(int i = 1; i <= tLastDay.Day; i++)
            {
                #region Set Calender
                switch (tWeekCnt)
                {
                    case 0:
                        FirstWeek.Controls[(int)tFirstDay_DayofWeek].Text = i.ToString();
                        FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        if (pIsToday && i == pDay) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.LightYellow;
                        else if (FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor == Color.LightYellow) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        FirstWeek.Controls[(int)tFirstDay_DayofWeek].MouseEnter += LabelMouseHoverIn_Event;
                        FirstWeek.Controls[(int)tFirstDay_DayofWeek].MouseLeave += LabelMouseHoverOut_Event;
                        break;

                    case 1:
                        SecondWeek.Controls[(int)tFirstDay_DayofWeek].Text = i.ToString();
                        SecondWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        if (pIsToday && i == pDay) SecondWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.LightYellow;
                        else if (SecondWeek.Controls[(int)tFirstDay_DayofWeek].BackColor == Color.LightYellow) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        SecondWeek.Controls[(int)tFirstDay_DayofWeek].MouseEnter += LabelMouseHoverIn_Event;
                        SecondWeek.Controls[(int)tFirstDay_DayofWeek].MouseLeave += LabelMouseHoverOut_Event;
                        break;

                    case 2:
                        TirdWeek.Controls[(int)tFirstDay_DayofWeek].Text = i.ToString();
                        TirdWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        if (pIsToday && i == pDay) TirdWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.LightYellow;
                        else if (TirdWeek.Controls[(int)tFirstDay_DayofWeek].BackColor == Color.LightYellow) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        TirdWeek.Controls[(int)tFirstDay_DayofWeek].MouseEnter += LabelMouseHoverIn_Event;
                        TirdWeek.Controls[(int)tFirstDay_DayofWeek].MouseLeave += LabelMouseHoverOut_Event;
                        break;

                    case 3:
                        FourWeek.Controls[(int)tFirstDay_DayofWeek].Text = i.ToString();
                        FourWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        if (pIsToday && i == pDay) FourWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.LightYellow;
                        else if (FourWeek.Controls[(int)tFirstDay_DayofWeek].BackColor == Color.LightYellow) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        FourWeek.Controls[(int)tFirstDay_DayofWeek].MouseEnter += LabelMouseHoverIn_Event;
                        FourWeek.Controls[(int)tFirstDay_DayofWeek].MouseLeave += LabelMouseHoverOut_Event;
                        break;

                    case 4:
                        FiveWeek.Controls[(int)tFirstDay_DayofWeek].Text = i.ToString();
                        FiveWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        if (pIsToday && i == pDay) FiveWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.LightYellow;
                        else if (FiveWeek.Controls[(int)tFirstDay_DayofWeek].BackColor == Color.LightYellow) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        FiveWeek.Controls[(int)tFirstDay_DayofWeek].MouseEnter += LabelMouseHoverIn_Event;
                        FiveWeek.Controls[(int)tFirstDay_DayofWeek].MouseLeave += LabelMouseHoverOut_Event;
                        break;

                    case 5:
                        SixWeek.Controls[(int)tFirstDay_DayofWeek].Text = i.ToString();
                        SixWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        if (pIsToday && i == pDay) SixWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.LightYellow;
                        else if (SixWeek.Controls[(int)tFirstDay_DayofWeek].BackColor == Color.LightYellow) FirstWeek.Controls[(int)tFirstDay_DayofWeek].BackColor = Color.WhiteSmoke;

                        SixWeek.Controls[(int)tFirstDay_DayofWeek].MouseEnter += LabelMouseHoverIn_Event;
                        SixWeek.Controls[(int)tFirstDay_DayofWeek].MouseLeave += LabelMouseHoverOut_Event;
                        break;
                }

                if (tFirstDay_DayofWeek == DayOfWeek_IDX.SATURDAY)
                {
                    tFirstDay_DayofWeek = DayOfWeek_IDX.SUNDAY;
                    tWeekCnt++;
                }
                else
                {
                    if (tFirstDay_DayofWeek == DayOfWeek_IDX.THURSDAY)
                        tFirstDay_DayofWeek = DayOfWeek_IDX.FRIDAY;
                    else tFirstDay_DayofWeek--;
                }
                #endregion
            }
        }

        public void ClearCalendar()
        {
            #region Clear
            foreach (Label lbl in FirstWeek.Controls)
            {
                lbl.BackColor = Color.FromArgb(250, 250, 250);
                lbl.Text = "";
                lbl.MouseEnter -= LabelMouseHoverIn_Event;
                lbl.MouseLeave -= LabelMouseHoverOut_Event;
            }

            foreach (Label lbl in SecondWeek.Controls)
            {
                lbl.BackColor = Color.FromArgb(250, 250, 250);
                lbl.Text = "";
                lbl.MouseEnter -= LabelMouseHoverIn_Event;
                lbl.MouseLeave -= LabelMouseHoverOut_Event;
            }

            foreach (Label lbl in TirdWeek.Controls)
            {
                lbl.BackColor = Color.FromArgb(250, 250, 250);
                lbl.Text = "";
                lbl.MouseEnter -= LabelMouseHoverIn_Event;
                lbl.MouseLeave -= LabelMouseHoverOut_Event;
            }

            foreach (Label lbl in FourWeek.Controls)
            {
                lbl.BackColor = Color.FromArgb(250, 250, 250);
                lbl.Text = "";
                lbl.MouseEnter -= LabelMouseHoverIn_Event;
                lbl.MouseLeave -= LabelMouseHoverOut_Event;
            }

            foreach (Label lbl in FiveWeek.Controls)
            {
                lbl.BackColor = Color.FromArgb(250, 250, 250);
                lbl.Text = "";
                lbl.MouseEnter -= LabelMouseHoverIn_Event;
                lbl.MouseLeave -= LabelMouseHoverOut_Event;
            }

            foreach (Label lbl in SixWeek.Controls)
            {
                lbl.BackColor = Color.FromArgb(250, 250, 250);
                lbl.Text = "";
                lbl.MouseEnter -= LabelMouseHoverIn_Event;
                lbl.MouseLeave -= LabelMouseHoverOut_Event;
            }
            #endregion
        }
        #endregion

        #region Dispose
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeAll();
        }

        private void DisposeAll()
        {
            IsRunningClock = false;
            Thread.Sleep(10);
            if(Clock.IsAlive) Clock.Abort();
        }
        #endregion
    }
}
