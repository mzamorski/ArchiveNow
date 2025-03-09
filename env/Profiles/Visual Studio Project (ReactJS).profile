{  
    "Name":"Visual Studio Project (ReactJS)",
    "CreateDate":"2017-01-18",
    "ModifyDate":"2017-01-18",
    "IgnoredDirectories":[  
        "^.svn$",
        "^.vs$",
        "^bin$",
        "^obj$",
        "^tmp$",
        "^packages$",
		"^node_modules$"
    ],
    "IgnoredFiles":[  
        "\\.tmp$",
        "\\.DotSettings.user$",
		"\\.(7z|zip|rar)$"
    ],
    "FileNameBuilder":"AddDateTime",
    "Password":"test",
    "UsePlainTextPasswords":true,
    "ProviderName":"SystemZip",
	"UseDefaultActionPrecedence": "true",
	"AfterFinishedActions":[  
		
        {
			"Name": "SendToMailBox",
			"Context": {
				"Host": "smtp.gmail.com",
				"Port": 587,
				"UserName": null,
				"Password": null,
				"Recipient": "marcin.zamorski@gmail.com",
				"Sender": "marcin.zamorski@gmail.com",
				"Subject": null
			}
		},
		
		{
			"Name": "SetClipboard"
		},
    ]
}