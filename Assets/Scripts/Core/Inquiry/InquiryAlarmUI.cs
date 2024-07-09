using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InquiryAlarmUI
{
    public string otime;
    public string ctime;
    public string cname;
    public string desc;

    public InquiryAlarmUI(string otime, string ctime, string cname, string desc)
    {
        this.otime = otime;
        this.ctime = ctime;
        this.cname = cname;
        this.desc = desc;
    }

    public InquiryAlarmUI()
    {
    }
}