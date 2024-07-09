using System;
using System.Data;
using UI.Dates;
using UIWidgets.Custom.InquiryControlUINS;
using UIWidgets.Examples;
using UnityEngine;
using UnityEngine.Serialization;

namespace Develop._01GeneralListGUI
{
    public class InquiryControlData : MonoBehaviour
    {
        /// <summary>
        /// SteamSpyView.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("steamSpyView")]
        protected ListViewInquiryControlUI _listViewInquiryControlUI;

        public static InquiryControlData Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            InquiryControl();
        }

        public void InquiryControl()
        {
            _listViewInquiryControlUI.DataSource.Clear();
            DateTime sDate = InquiryManager.Instance.objStartDatePicker.GetComponent<DatePicker>().SelectedDate;
            sDate = sDate.Date; // This sets the time to 00:00:00

            DateTime eDate = InquiryManager.Instance.objEndDatePicker.GetComponent<DatePicker>().SelectedDate;
            eDate = eDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // This sets the time to 23:59:59

            DataTable dataTable = ClientDatabase.FetchControllingData(sDate, eDate, "all", "all").Tables[0];
            string atime = string.Empty;
            string cname = string.Empty;
            string desc = string.Empty;
            string ctlUser = string.Empty;

            foreach (DataRow row in dataTable.Rows)
            {
                atime = row["ATIME"].ToString();
                cname = row["CNAME"].ToString();
                desc = row["DESC"].ToString();
                ctlUser = row["CTL_USER"].ToString();

                InquiryControlUI _inquiryControlUI = new InquiryControlUI(atime, cname, desc, ctlUser);
                _listViewInquiryControlUI.DataSource.Add(_inquiryControlUI);
            }
        }

        public void InquirySpecificControl(string iid, string cid)
        {
            _listViewInquiryControlUI.DataSource.Clear();
            DateTime sDate = InquiryManager.Instance.objStartDatePicker.GetComponent<DatePicker>().SelectedDate;
            DateTime eDate = InquiryManager.Instance.objEndDatePicker.GetComponent<DatePicker>().SelectedDate;

            DataTable dataTable = ClientDatabase.FetchControllingData(sDate, eDate, iid, cid).Tables[0];
            string atime = string.Empty;
            string cname = string.Empty;
            string desc = string.Empty;
            string ctlUser = string.Empty;

            foreach (DataRow row in dataTable.Rows)
            {
                atime = row["ATIME"].ToString();
                cname = row["CNAME"].ToString();
                desc = row["DESC"].ToString();
                ctlUser = row["CTL_USER"].ToString();

                InquiryControlUI _inquiryControlUI = new InquiryControlUI(atime, cname, desc, ctlUser);
                _listViewInquiryControlUI.DataSource.Add(_inquiryControlUI);
            }
        }
    }
}

