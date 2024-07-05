{  
    "Name":"NUMIZMATYKA",
    "CreateDate":"2017-01-18",
    "ModifyDate":"2017-01-18",
    "IgnoredDirectories":[  
        "^.svn$",
		"^EDYTOWANE$",
		"^FILMY$",
		"^TE?MP$",
		"^__SPRZEDA",
		"^__$",
    ],
    "IgnoredFiles":[  
		"\\.mkv$",
		"\\.afphoto$"
    ],
    "FileNameBuilder":"AddDateTime",
    "Password":null,
    "UsePlainTextPasswords":false,
    "ProviderName":"SevenZip",
	"UseDefaultActionPrecedence": "true",
	"BreakActionsIfError": "true",
	"AfterFinishedActions":[  
		
		{
			"Name": "SetClipboard"
		},
		{
			"Name": "MoveToDirectory",
			"Context": {
				"Path": "D:\\BACKUP",
			}
		},
    ]
}