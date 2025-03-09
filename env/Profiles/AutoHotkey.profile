{  
    "Name":"AutoHotkey",
    "CreateDate":"2019-01-04",
    "ModifyDate":"2019-01-04",
    "IgnoredDirectories":[ 
		"^((?!libs).)*$"
	],
    "IgnoredFiles":[  
        "^(.(?!.*\\.ahk))*$"
    ],
    "FileNameBuilder":"AddDateTime",
    "Password":null,
    "UsePlainTextPasswords":false,
    "ProviderName":"LZ4",
	"UseDefaultActionPrecedence": "true",
	"BreakActionsIfError": "true",
	"AfterFinishedActions":[  
		{
			"Name": "SetClipboard"
		},
		{
			"Name": "MoveToDirectory",
			"Context": {
				"Path": "C:\\TEMP",
			}
		},
    ]
}