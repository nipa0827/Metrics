# -*- coding: utf-8 -*-
"""
Created on Tue Oct 15 20:59:31 2019

@author: nipa
"""

import re

def findPublicClass(file_content):
    counter = 0
    for line in file_content:
        valid = False
        valid = isPublicClass(line)
            
        if valid:
            counter +=1
            
    return counter

def findAllClass(file_content):
    counter = 0
    for line in file_content:
        valid = False
        valid = isClass(line)
            
        if valid:
            counter +=1
            
    return counter
                
def getClassName(line):
    line = line.strip() 
    keywords = line.split(" ")
    
    if (keywords.len >= 2 and keywords[1]=="class" and keywords[0]=="public"):
        return keywords[2]


def isPublicClass(line):
    line = line.strip()
    if line.startswith('#'):
        return False
    if line.startswith('//'):
        return False
   
    if ("class" in line) and ("public" in line):
       print(line + " end")
       return True
   
    return False    

def isClass(line):
    line = str(line)
    
    line = line.strip()

    if line.startswith('#'):
        return False
    if line.startswith('//'):
        return False
   
    if ("class" in line):
       print(line + " end")
       return True
   
    return False    

def getAllAttributesAndMethods(content):
    public_methods = 0
    private_methods = 0
    private_attributes = 0
    public_attributes = 0
    protected_attributes = 0
    
    
    
    
    for line in content:
        str_line = str(line)
        line = line.split(" ")
        
        if (line[0]=="public" or line[0]=="private" or line[0]=="protected" ):
            
            if str_line.endswith(";"):
                if not str_line.endswith(");"):
                    if line[0]=="public":
                        public_attributes += 1
                    if line[0] == "private":
                        private_attributes += 1
                    if line[0]=="protected":
                        protected_attributes += 1
                        
            if str_line.endswith(")") or str_line.endswith(");"):
                if line[0]=="public":
                    public_methods += 1
                if line[0]=="private":
                    private_methods += 1
                    
    return public_attributes, private_attributes, protected_attributes, public_methods, private_methods

def getAllClass(file_content):  
    public_classes = findPublicClass(file_content)
    all_classes = findAllClass(file_content)
    return public_classes, all_classes




    