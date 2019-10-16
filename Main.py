# -*- coding: utf-8 -*-
"""
Created on Wed Oct 16 13:09:32 2019

@author: nipa
"""

from FileRead import getAllResult
import csv

filename = "Result.csv"
header = ("Project Name", "Github Link", "Line of Code", "TC", "WMC", "NOC", "MHF", "AHF", "AHF", "MIF", "AIF", "LCOM","COF", "COB", "POF")

project_name = ["Banking System", "Hospitalization Charges Application", "Bug Tracking Application", "Barcode Reader Library", "Huffman Encoding", 
                "Ebay Oauth Authentication", "Task Management System", "Dental Clinic Management System","Invoice Managment System", "Cafe Management" ]
github_link = ["https://github.com/meyashtiwari/Banking-System", "https://github.com/ajayrandhawa/Hospitalization-Charges-Application-Assignment", 
               "https://github.com/vijaythapa333/BugTrackingApplication", "https://github.com/barnhill/barcodelib", "https://github.com/arslanbilal/Huffman-Code-CSharp", 
               "https://github.com/eBay/ebay-oauth-csharp-client", "https://github.com/khaledkucse/TaskManagementSystem","https://github.com/ddevilred1/",
               "https://github.com/iammemon/Invoice_ManagmentSystem", "https://github.com/kanee98/Cafe_Management"]
  

project_path = ["projects\\banking_system\\", "projects\\hospitalization_charges\\", "projects\\bug_tracking\\", "projects\\barcode_reader", "projects\\huffman_code\\", 
                "projects\\ebay_authentication\\", "projects\\task_management\\", "projects\\dental_clinic_management\\DCMS", "projects\\invoice_management\\invoice_ms\\invoice_ms",
                "projects\\cafe_management\\Cafe_Management_System"]

with open (filename, "w", newline = "") as csvfile:
    result = csv.writer(csvfile)
    result.writerow(header)

    for i in range(len(project_path)):
        line_of_code, TC, WMC, NOC, MHF, AHF, MIF, AIF, LCOM, COF, COB, POF = getAllResult(project_path[i])
        
        lst = [project_name[i], github_link[i] ,line_of_code, TC, WMC, NOC, MHF, AHF, MIF, AIF, LCOM, COF, COB, POF]
        
        result.writerow(lst)
    print(getAllResult(project_path[i]))

csvfile.close()