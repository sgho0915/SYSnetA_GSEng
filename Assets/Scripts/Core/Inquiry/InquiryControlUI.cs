using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InquiryControlUI
{
    public string atime;
    public string cname;
    public string desc;
    public string ctlUser;

    public InquiryControlUI(string atime, string cname, string desc, string ctlUser)
    {
        this.atime = atime;
        this.cname = cname;
        this.desc = desc;
        this.ctlUser = ctlUser;
    }

    public InquiryControlUI()
    {
    }
}