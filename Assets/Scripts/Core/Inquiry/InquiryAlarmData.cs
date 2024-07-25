using System;
using System.Data;
using UI.Dates;
using UIWidgets.Custom.InquiryAlarmUINS;
using UIWidgets.Examples;
using UnityEngine;
using UnityEngine.Serialization;

namespace Develop._01GeneralListGUI
{
    public class InquiryAlarmData : MonoBehaviour
    {
        /// <summary>
        /// SteamSpyView.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("steamSpyView")]
        protected ListViewInquiryAlarmUI _listViewInquiryAlarmUI;

        public static InquiryAlarmData Instance { get; private set; }

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
            InquiryAlarm();
        }

        public void InquiryAlarm()
        {
            _listViewInquiryAlarmUI.DataSource.Clear();
            DateTime sDate = InquiryManager.Instance.objStartDatePicker.GetComponent<DatePicker>().SelectedDate;
            sDate = sDate.Date; // This sets the time to 00:00:00

            DateTime eDate = InquiryManager.Instance.objEndDatePicker.GetComponent<DatePicker>().SelectedDate;
            eDate = eDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // This sets the time to 23:59:59


            DataTable dataTable = ClientDatabase.FetchWarningData(sDate, eDate, "all", "all").Tables[0];
            string otime = string.Empty;
            string ctime = string.Empty;
            string cname = string.Empty;
            string desc = string.Empty;

            foreach (DataRow row in dataTable.Rows)
            {
                otime = row["OCCUR_TIME"].ToString();
                ctime = row["UNSET_TIME"].ToString();
                cname = row["CNAME"].ToString();
                desc = row["DESC"].ToString();

                InquiryAlarmUI _inquiryAlarmUI = new InquiryAlarmUI(otime, ctime, cname, desc);
                _listViewInquiryAlarmUI.DataSource.Add(_inquiryAlarmUI);
            }
        }

        public void InquirySpecificAlarm(string iid, string cid)
        {
            _listViewInquiryAlarmUI.DataSource.Clear();
            DateTime sDate = InquiryManager.Instance.objStartDatePicker.GetComponent<DatePicker>().SelectedDate;
            DateTime eDate = InquiryManager.Instance.objEndDatePicker.GetComponent<DatePicker>().SelectedDate.Date.AddDays(1).AddSeconds(-1);

            DataTable dataTable = ClientDatabase.FetchWarningData(sDate, eDate, iid, cid).Tables[0];
            string otime = string.Empty;
            string ctime = string.Empty;
            string cname = string.Empty;
            string desc = string.Empty;

            if (dataTable.Rows.Count < 1)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "��ȸ ����� �����ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
            }

            foreach (DataRow row in dataTable.Rows)
            {
                otime = row["OCCUR_TIME"].ToString();
                ctime = row["UNSET_TIME"].ToString();
                cname = row["CNAME"].ToString();
                desc = row["DESC"].ToString();

                Debug.Log($"{otime}, {ctime}, {cname}, {desc}");

                InquiryAlarmUI _inquiryAlarmUI = new InquiryAlarmUI(otime, ctime, cname, desc);
                _listViewInquiryAlarmUI.DataSource.Add(_inquiryAlarmUI);
            }
        }
    }
}

