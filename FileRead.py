# -*- coding: utf-8 -*-
"""
Created on Tue Oct 15 19:45:41 2019

@author: nipa
"""
import os
import glob
import io
from FindProperties import getAllClass, getAllAttributesAndMethods

import re

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
            
obj = FileOperation()
content = obj.readFileContent("calculator\\")
final_content = []

for line in content:
    line = line.strip()
    final_content.append(line)
    
    
public_classes, all_classes = getAllClass(final_content)

public_attributes, private_attributes, protected_attributes, public_methods, private_methods = getAllAttributesAndMethods(final_content)