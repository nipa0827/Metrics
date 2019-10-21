dim shl, fso	' some global objects
dim sPath, dDate, i, oldVersion, newVersion, daysOfYear

if WScript.Arguments.Count > 0 then		
	i = 0
	do while i < WScript.Arguments.Count
		s = WScript.Arguments.Item(i)
		if left(s,1) = "-" then		' it's an option
			select case ucase(mid(s,2,1))
				case "?", "h":	Wscript.Echo "usage: GenerateVersionInfo <fullpathname to versioninfo file>"
				case else
					WScript.Echo "Unkonwn option ignored -> '" & s & "'"
			end select
		else	' file path
			sPath = s
		end if
		i = i + 1
	loop
else
	WScript.Echo "Missing argument: path and filename of VersionInfo.cs"
	WScript.Quit 1
end if

Set shl = WScript.CreateObject( "WScript.Shell" )
if shl is nothing then
  WScript.Echo "Could not create 'WScript.Shell'."
  WScript.Quit 2
end if
set fso = WScript.CreateObject("Scripting.FileSystemObject")
if fso is nothing then
  WScript.Echo "Could not create 'Scripting.FileSystemObject'."
  WScript.Quit 3
end if

daysOfYear = DatePart("y", Now)
do while Len(daysOfYear) < 3
  daysOfYear = "0" & daysOfYear
loop
dDate = Right(CStr(Year(Now)), 1) & daysOfYear
oldVersion = ExtractOldVersion(sPath)
WScript.Echo "Found/Generated old version: " & oldVersion
newVersion = Mid(oldVersion, 1, InStrRev(oldVersion, "."))
newVersion = newVersion & dDate
WScript.Echo "New version created: " & newVersion

WriteInfoWithVersion sPath, newVersion
' that's it

' helper functions
function ExtractOldVersion(file) 
	ExtractOldVersion = "1.0.0."
	if fso.FileExists(file) then
		Dim fo, content, verpos, verend, c
		set fo = fso.OpenTextFile(file, 1)
		content = fo.ReadAll
		fo.Close
		verpos = InStr(content, "AssemblyVersion(") 
		if verpos > 0 then
			verpos = verpos + 16
			do while Not IsNumeric(Mid(content, verpos, 1))
				verpos = verpos + 1
			loop
			verend = verpos + 1
			c = Mid(content, verend, 1) 
			do while IsNumeric(c) Or c = "." Or c = "*"
				verend = verend + 1
				c = Mid(content, verend, 1) 
			loop
			ExtractOldVersion = replace(mid(content, verpos, verend - verpos), "*", "0")
		end if
	end if
end function

sub WriteInfoWithVersion(file, versionStr)
	Dim s, sQuote, fo
	sQuote = """"
	s = "[assembly: AssemblyVersion(" & sQuote & versionStr & sQuote & ")]"
	set fo = fso.OpenTextFile(file, 2, true)
	fo.WriteLine "// Autogenerated file!"
	fo.WriteLine "// Any changes you apply to revision number part will be overwritten next time the tool runs!"
	fo.WriteLine "// Changes to the major, minor and build number will be preserved."
	fo.WriteLine "// Revision Number is autogenerated as: 1 digit current year and 3 digits day of current year."
	fo.WriteLine ""
	fo.WriteLine "using System.Reflection;"
	fo.WriteLine "using System.Runtime.CompilerServices;"
	fo.WriteLine ""
	fo.WriteLine s
	fo.Close
end sub


