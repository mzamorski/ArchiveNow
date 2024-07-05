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
    "Password":"ronin.135",
    "UsePlainTextPasswords":true,
    "ProviderName":"SystemZip",
	"UseDefaultActionPrecedence": "true",
	"AfterFinishedActions":[  
		
        {
			"Name": "SendToMailBox",
			"Context": {
				"Host": "pl-ex01.kruk-inkaso.com.pl",
				"Port": null,
				"UserName": null,
				"Password": null,
				"Recipient": "marcin.zamorski@kruksa.pl",
				"Sender": "marcin.zamorski@kruksa.pl",
				"Subject": null
			}
		},
		
		{
			"Name": "SetClipboard"
		},
    ]
}