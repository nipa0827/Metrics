# -*- coding: utf-8 -*-
"""
Created on Tue Oct 15 20:59:31 2019

@author: nipa
"""

import re
import math


dataType = ["bool", "void", "String[]", "double", "float"]
all_class_name = []
name = []

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
       return True
   
    return False    

def isClass(line):
    line = str(line)
    class_name = line
    line = line.strip()

    if line.startswith('#'):
        return False
    if line.startswith('//'):
        return False
    
    if ("class" in line):
        all_class_name.append(class_name)
        return True
   
    return False    

def getAllAttributesAndMethods(content):
    public_methods = 0
    private_methods = 0
    private_attributes = 0
    public_attributes = 0
    protected_attributes = 0
    
    method_name = []
    
    
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
                    regexp = re.compile("public(.*)$")
                    method = (regexp.search(str_line).group(1))
                    method = method.split( " ")
                    
                    if method[1] not in dataType:
                        method_name.append(method[1])
                        
                    else:
                        method_name.append(method[2])
                    
                    #method_name.append(method[1])
                    public_methods += 1
                if line[0]=="private":
                    regexp = re.compile("private(.*)$")
                    method = (regexp.search(str_line).group(1)).split(" ")
                    if method[1] not in dataType:
                        method_name.append(method[1])
                        
                    else:
                        method_name.append(method[2])
                        
                    private_methods += 1
                
                
                
                    
    return public_attributes, private_attributes, protected_attributes, public_methods, private_methods, method_name

def getAllClass(file_content):  
    public_classes = findPublicClass(file_content)
    all_classes = findAllClass(file_content)
    return public_classes, all_classes, all_class_name


def getPolymorphicMethod(method_name):
    polymorphic_method = (set([x for x in method_name if method_name.count(x) > 1]))
    
    #print(polymorphic_method)
    
    return polymorphic_method
    
def findDescendants(all_class_name):
    descendants = 0
    for line in all_class_name:
        keywords = line.split(" ")
		
        if ":" in keywords:
            #print(line)
            descendants += 1
            
    return descendants
    
    
def findAllClassName(class_name):
    for line in class_name:  
        line = str(line)
        line = line.split("class")
        newLine = line[1]
        
        newLine = newLine.split(" ")
        name.append(newLine[1])
    
    return name
    
    
def findAllCall(class_name, content):
    
    total_call = 0
    total_call_class = 0
    
    for cls_n in class_name:
        found = False
        
        for line in content:
            if cls_n in line:
                found = True
                total_call += 1
					
			
        if found:
            total_call_class += 1
            
    return total_call, total_call_class	
	
    
def findLCOM(content, method_name):
    LCOM_before = nCr(len(method_name), 2)
      
    LCOM = LCOM_before
    
    for method in method_name:
        for line in content:
            line = line.strip()
        
            if method in line: 
                LCOM -= 1
    
    
    
    return LCOM/LCOM_before
   
def nCr(n, r):
    result = math.factorial(n)/(math.factorial(n-r)*(math.factorial(r)))
    
    return result
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    