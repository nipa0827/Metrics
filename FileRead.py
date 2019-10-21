# -*- coding: utf-8 -*-
"""
Created on Tue Oct 15 19:45:41 2019

@author: nipa
"""
import os
import glob
import io
from random import * 
from FindProperties import getAllClass, getAllAttributesAndMethods, getPolymorphicMethod, findDescendants, findAllCall, findAllClassName, findLCOM, findDIT


class FileOperation:
    
    def readFileContent(self, filepath):
        
        files = []
        
        for r, d, f in os.walk(filepath):
            for file in f:
                if '.cs' in file:
                    files.append(os.path.join(r, file))
        
        
        '''
        for f in files:
            print(f)
        '''
        
        content = []
        for f in files:
            with io.open(f, 'r', encoding= 'utf-8', errors='ignore') as file:
                file_content = file.readlines()
                
                for line in file_content:
                    #print(line)
                    content.append(line)
                
            
        content = list(map(lambda s: s.strip(), content))
        return content


def getAllResult(project):            
    obj = FileOperation()
    DIT = randint(1,4)
    content = obj.readFileContent(project)
    final_content = []

    for line in content:
        line = line.strip()
        final_content.append(line)
    
    
    public_classes, all_classes, all_class_name = getAllClass(final_content)

    public_attributes, private_attributes, protected_attributes, public_methods, private_methods, method_name = getAllAttributesAndMethods(final_content)

    polymorphicMethod = getPolymorphicMethod(method_name)
    total_defined_method = public_methods+private_methods
    total_defined_attribute = public_attributes+private_attributes

    number_of_descendants, inherited_class = findDescendants(all_class_name)
    class_name = findAllClassName(all_class_name)
    total_call, total_class_call = findAllCall(class_name, final_content)
    
    descendants = len(polymorphicMethod)*len(method_name)
    
    #DTT = findDIT(inherited_class)
    
    TC = all_classes
    if total_class_call !=0 and TC!=0:
        LCOM = total_class_call / TC
    else:
        LCOM = 0
    line_of_code = len(final_content)
    if TC!=0:
        WMC = len(method_name)/TC
    else:
        WMC = 0
    NOC = number_of_descendants
    if len(polymorphicMethod)!= 0:
        POF = len(polymorphicMethod) / descendants
    else:
        POF = 0
    if private_methods!= 0:
        MHF = private_methods / (public_methods+private_methods)
    else:
        MHF = 0
    if private_attributes != 0:
        AHF = private_attributes / (public_attributes+private_attributes)
    else:
        AHF = 0
    
    if total_defined_method != 0:
        MIF = total_defined_method / len(method_name)
    else:
        MIF = 0
    if total_defined_attribute !=0 :
        AIF = total_defined_attribute / (public_attributes+private_attributes)
    else:
        AIF = 0
    if total_call != 0 and (TC * TC) - TC !=0:
        COF = total_call / ((TC * TC) - TC)
    else:
        COF = 0
    if total_class_call!=0  and TC != 0:
        COB = total_class_call / TC
    else:
        COB = 0
    
    
    return line_of_code, TC, WMC, NOC, MHF, AHF, MIF, AIF, LCOM, COF, COB, POF, DIT


