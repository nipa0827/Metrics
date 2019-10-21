# -*- coding: utf-8 -*-
"""
Created on Wed Oct 16 13:09:32 2019

@author: nipa
"""

from FileRead import getAllResult
import csv

filename = "Result.csv"
header = ("Project Name", "Github Link", "Line of Code", "TC", "WMC", "NOC", "MHF", "AHF", "MIF", "AIF", "LCOM","COF", "COB", "POF", "DIT")

project_name = ["Banking System", "Bug Tracking Application", "Barcode Reader Library", "Huffman Encoding", 
                "Ebay Oauth Authentication", "Task Management System", "Dental Clinic Management System","Invoice Managment System", "Bug Tracker",
                "iText Shaprt", "RSS BandIt" ]
github_link = ["https://github.com/meyashtiwari/Banking-System", 
               "https://github.com/vijaythapa333/BugTrackingApplication", "https://github.com/barnhill/barcodelib", "https://github.com/arslanbilal/Huffman-Code-CSharp", 
               "https://github.com/eBay/ebay-oauth-csharp-client", "https://github.com/khaledkucse/TaskManagementSystem","https://github.com/ddevilred1/",
               "https://github.com/iammemon/Invoice_ManagmentSystem", "https://sourceforge.net/projects/btnet/", 
               "https://github.com/itext/itextsharp", "https://github.com/RssBandit/RssBandit"]
  

project_path = ["C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\banking_system\\",
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\bug_tracking\\", 
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\barcode_reader",
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\huffman_code\\HuffmanCodeWithCSharp\\", 
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\ebay_authentication\\", 
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\task_management\\", 
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\dental_clinic_management\\DCMS",
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\invoice_management\\invoice_ms\\invoice_ms",
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\btnet\\",
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\itextsharp\\src\\", 
                "C:\\Users\\nipa1\\Desktop\\Metrics\\projects\\rss_bandit\\"]

with open (filename, "w", newline = "") as csvfile:
    result = csv.writer(csvfile)
    result.writerow(header)

    for i in range(len(project_path)):
        line_of_code, TC, WMC, NOC, MHF, AHF, MIF, AIF, LCOM, COF, COB, POF, DIT = getAllResult(project_path[i])
        
        lst = [project_name[i], github_link[i] ,line_of_code, TC, WMC, NOC, MHF, AHF, MIF, AIF, LCOM, COF, COB, POF, DIT]
        
        result.writerow(lst)
    #print(getAllResult(project_path[i]))

csvfile.close()