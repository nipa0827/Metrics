# -*- coding: utf-8 -*-
"""
Created on Tue Oct 15 19:45:41 2019

@author: nipa
"""
import os
import glob
import io
from FindProperties import getAllClass, getAllAttributesAndMethods, getPolymorphicMethod, findDescendants, findAllCall, findAllClassName, findLCOM


class FileOperation:
    
    def readFileContent(self, filepath):
        
        files = [f for f in glob.glob(filepath + "**/*.cs", recursive=True)]
        
        
        '''
        for f in files:
            print(f)
        '''
        
        content = []
        for f in files:
            with io.open(f, 'r') as file:
                file_content = file.readlines()
                
                for line in file_content:
                    content.append(line)
                
            
        content = list(map(lambda s: s.strip(), content))
        return content


def getAllResult():            
    obj = FileOperation()
    project = ["Calculator"]
    github_link = ["bla"]
    content = obj.readFileContent("calculator\\")
    final_content = []

    for line in content:
        line = line.strip()
        final_content.append(line)
    
    
    public_classes, all_classes, all_class_name = getAllClass(final_content)

    public_attributes, private_attributes, protected_attributes, public_methods, private_methods, method_name = getAllAttributesAndMethods(final_content)

    polymorphicMethod = getPolymorphicMethod(method_name)
    total_defined_method = public_methods+private_methods
    total_defined_attribute = public_attributes+private_attributes
    project_name =  project[0]

    number_of_descendants = findDescendants(all_class_name)
    class_name = findAllClassName(all_class_name)
    total_call, total_class_call = findAllCall(class_name, final_content)
    
    descendants = len(polymorphicMethod)*len(method_name)
    
    TC = all_classes
    LCOM = total_class_call / TC
    line_of_code = len(final_content)
    WMC = len(method_name)/TC
    NOC = number_of_descendants
    if len(polymorphicMethod)!= 0:
        POF = len(polymorphicMethod) / descendants
    else:
        POF = 0
    MHF = private_methods / (public_methods+private_methods)
    AHF = private_attributes / (public_attributes+private_attributes)
    MIF = total_defined_method / len(method_name);
    AIF = total_defined_attribute / (public_attributes+private_attributes)
    COF = total_call / (TC * TC - TC)
    COB = total_class_call / TC;
    
    
    return line_of_code, TC, WMC, NOC, MHF, AHF, MIF, AIF, LCOM, COF, COB, POF


