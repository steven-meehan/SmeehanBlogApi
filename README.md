# SmeehanBlogApi

## Dependancies
- [.Net Core 3.1](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-1)
- [AWS Toolkit for Visual Studio for 2017 and 2019](https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/welcome.html) version 1.21.2.0.
- [AWS Lambda](https://docs.aws.amazon.com/lambda/index.html)
- [AWS DynamoDB](https://docs.aws.amazon.com/dynamodb/)

## Purpouse
AWS Lambda Serverless Application to power the dynamic widgets for stevenmeehan.com.

## Local Configuration

After installing the AWS Toolkit for Visual 2017 and 2019 follow the instructions 
[here](https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/keys-profiles-credentials.html). 
Follow Amazon's instructions to provide your user with the neccessary permissions to work with the required 
resources.

Configure user secrets for the `DynamoDBOptions` object.

```JSON
{
  "DynamoDB": {
    "AccessKey" : "VALUE PULLED FROM AWS IAM",
    "SecretKey" : "VALUE PULLED FROM AWS IAM"
  }
}
```

## APIs

This application powers two dynamic widgets, Quotes and Works in Progress, each with their own endpoint.

### Quotes

This requires the pre-existing Quotes table, as defined in the `Quote.cs` file. It works with that existing 
table to power the Quotes widget on Steven Meehan's [Blog](https://stevenmeehan.com).

#### Methods

- [Random Quotes](#random-quotes)
- [Get by Id](#get-quote-by-id)

##### Random Quotes

- **URL:** `/api/quote/random/{numberOfQuotes}`

- **Method:** `GET`

- **Parameters:**

	- Path Parameter: `numberOfQuotes` (int, required)
		
		Specifies the number of Quotes to pull from the DynamoDB table.

- **JSON Sample Response**

    [
        {
            "id": 1013,
            "speakers": [
                {
                    "person": "Will Turner",
                    "words": "This is either madness or brilliance.",
                    "order": 1
                },
                {
                    "person": "Captain Jack Sparrow",
                    "words": "It's remarkable how often those two traits coincide.",
                    "order": 2
                }
            ],
            "source": {
                "story": "Pirates of the Caribbean: The Curse of the Black Pearl",
                "series": null
            }
        },
        .
        . (other results omitted for brevity)
        .
        {
            "id": 1291,
            "speakers": [
                {
                    "person": "Kvothe",
                    "words": "I'm to be whipped and admitted to the Arcanum.",
                    "order": 1
                },
                {
                    "person": "Simmon",
                    "words": "I'm sorry. Congratulations. Do I buy you a bandage or a beer?",
                    "order": 1
                }
            ],
            "source": {
                "story": "The Name of the Wind",
                "series": "The Kingkiller Chronicle"
            }
        }
    ]

##### Get Quote by Id

- **URL:** `/api/quote/{id}`

- **Method:** `GET`

- **Parameters:**

	- Path Parameter: `id` (int, required)
		
		Specifies the quote to retrieve from the DynamoDB table by its Id.
		
- **JSON Sample Response**
    
    {
        "id": 1084,
        "speakers": [
            {
                "person": "The Doctor",
                "words": "Ricky, let me tell you something about the human race. You put a mysterious blue box slap bang in the middle of town, what do they do? Walk past it, now stop your nagging. Let's go on and explore.",
                "order": 1
            }
        ],
        "source": {
            "story": "Doctor Who",
            "series": null
        }
    }

### Progress

This requires the pre-existing Progress table, as defined in the `Project.cs` file. It works with that 
existing table to power the Works in Progress widget on Steven Meehan's [Blog](https://stevenmeehan.com).

#### Methods

- [Active Works](#active-works-in-progress)

##### Active Works in Progress

- **URL:** `/api/progress/active`

- **Method:** `GET`

- **Parameters:** none

- **JSON Sample Response**

    [
        {
            "id": 1002,
            "active": true,
            "title": "Knavish",
            "type": 2,
            "series": null,
            "status": 4
        },
        .
        . (other results omitted for brevity)
        .
        {
            "id": 1009,
            "active": true,
            "title": "NaNoWriMo Project",
            "type": 2,
            "series": null,
            "status": 1
        }
    ]
