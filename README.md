# PrivateAI - The Privacy Layer

This is an unofficial wrapper for the [PrivateAI](https://www.private-ai.com) API.

## Configuration

Configuration keys to set:
- PrivateAi:ApiKey
- PrivateAi:Url

The Url can be set in the `appsettings.json` file. Possible values are:
- `https://api.private-ai.com/community/v4/` for the community version
- `https://api.private-ai.com/professional/v4/` for the professional version

```json
"PrivateAi": {
    "Url": "https://api.private-ai.com/community/v4/"
}
```

The ApiKey has to be set securely. For development you can use [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows). For production use your favorite key provider like [Azure Key Vault](https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-9.0)

```bash
cd FolderWithCsproj
dotnet user-secrets init
dotnet user-secrets set "PrivateAi:ApiKey" "<your-api-key>"
```

## Usage

Add all servies to the DI container.

```csharp
builder.Services.AddPrivateAi(builder.Configuration);
```

Resolve `IPrivateAiClient` however you like from the container.

```csharp
var privateAiClient = app.Services.GetRequiredService<IPrivateAiClient>();

var processTextRequest = new ProcessTextRequest
{
    Text = ["My name is Dominik and I was born on 02.03.1995. I was a software developer at Miele. Now I work as a freelancer."],
};

var result = (await privateAiClient.ProcessTextAsync(processTextRequest)).First();

Console.WriteLine(result.ProcessedText);
foreach (var (entity, index) in result.Entities.Select((x, i) => (x, i)))
{
    Console.WriteLine($"Redaction marker {index}: {entity.ProcessedText} | Original text: {entity.Text}");
}
```

Will output:
```text
My name is [NAME_GIVEN_1] and I was born on [DOB_1]. I was a [OCCUPATION_1] at [ORGANIZATION_1]. Now I work as a [OCCUPATION_2].
Redaction marker 0: NAME_GIVEN_1 | Original text: Dominik
Redaction marker 1: DOB_1 | Original text: 02.03.1995
Redaction marker 2: OCCUPATION_1 | Original text: software developer
Redaction marker 3: ORGANIZATION_1 | Original text: Miele
Redaction marker 4: OCCUPATION_2 | Original text: freelancer
```

### Enabling / Disabling entities

```csharp
var processTextRequest = new ProcessTextRequest
{
    Text = ["My name is Dominik and I was born on 02.03.1995. I was a software developer at Miele. Now I work as a freelancer."],
    EntityDetection = new EntityDetection
    {
        EntityTypes =
        [
            EntityTypeSelector.Enable(["NAME", "NAME_GIVEN", "DOB"]),
            EntityTypeSelector.Disable(["OCCUPATION", "ORGANIZATION"]),
        ],
    },
};
```

```text
My name is [NAME_GIVEN_1] and I was born on [DOB_1]. I was a software developer at Miele. Now I work as a freelancer.
Redaction marker 0: NAME_GIVEN_1 | Original text: Dominik
Redaction marker 1: DOB_1 | Original text: 02.03.1995
```

You can use legislations as entity types, too. It's a bit hidden in the documentation. Here are all values as of 2025-01-16

```text
// Legislations
"APPI", "APPI_SENSITIVE", "CORE_ENTITIES", "CPRA", "GDPR", "GDPR_SENSITIVE", "HEALTH_INFORMATION",
"HIPAA_SAFE_HARBOR", "LIDI", "PCI", "QUEBEC_PRIVACY_ACT", "CCI", "NUMERICAL_EXCL_PCI",

// Entity Types
"ACCOUNT_NUMBER", "AGE", "DATE", "DATE_INTERVAL", "DOB", "DRIVER_LICENSE", "DURATION", "EMAIL_ADDRESS",
"EVENT", "FILENAME", "GENDER", "SEXUALITY", "HEALTHCARE_NUMBER", "IP_ADDRESS", "LANGUAGE", "LOCATION",
"LOCATION_ADDRESS", "LOCATION_ADDRESS_STREET", "LOCATION_CITY", "LOCATION_COORDINATE", "LOCATION_COUNTRY",
"LOCATION_STATE", "LOCATION_ZIP", "MARITAL_STATUS", "MONEY", "NAME", "NAME_FAMILY", "NAME_GIVEN",
"NAME_MEDICAL_PROFESSIONAL", "NUMERICAL_PII", "OCCUPATION", "ORGANIZATION", "ORGANIZATION_MEDICAL_FACILITY",
"ORIGIN", "PASSPORT_NUMBER", "PASSWORD", "PHONE_NUMBER", "PHYSICAL_ATTRIBUTE", "POLITICAL_AFFILIATION", "RELIGION",
"SSN", "TIME", "URL", "USERNAME", "VEHICLE_ID", "ZODIAC_SIGN", "BLOOD_TYPE", "CONDITION", "DOSE", "DRUG",
"INJURY", "MEDICAL_PROCESS", "STATISTICS", "BANK_ACCOUNT", "CREDIT_CARD", "CREDIT_CARD_EXPIRATION", "CVV", "ROUTING_NUMBER"
```

### Additional filters

```csharp
var processTextRequest = new ProcessTextRequest
{
    Text = [_text],
    EntityDetection = new EntityDetection
    {
        Filter = [
            Filter.Allow(@"\b(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.(\d{4})\b"),
            Filter.Block("PHYSICAL_ATTRIBUTE", @"\bMy name\b", 1),
            Filter.AllowText(@"I was a.*?as a freelancer"),
        ],
    },
};
```

```text
[PHYSICAL_ATTRIBUTE_1] is [NAME_GIVEN_1] and I was born on 02.03.1995. I was a software developer at Miele. Now I work as a freelancer.
Redaction marker 0: PHYSICAL_ATTRIBUTE_1 | Original text: My name
Redaction marker 1: NAME_GIVEN_1 | Original text: Dominik
```

### Generating synthetic data instead of redaction markers

```csharp
var processTextRequest = new ProcessTextRequest
{
    Text = ["My name is Dominik and I was born on 02.03.1995. I was a software developer at Miele. Now I work as a freelancer."],
    ProcessedText = ProcessedText.Synthetic(SyntheticEntityAccuracy.StandardAutomatic, true, CoreferenceResolutionMode.Heuristics),
};
```

```text
My name is Michael and I was born on 11.01.1983. I was a software developer at Linux. Now I work as a programmer.
Redaction marker 0: Michael | Original text: Dominik
Redaction marker 1: 11.01.1983 | Original text: 02.03.1995
Redaction marker 2: software developer | Original text: software developer
Redaction marker 3: Linux | Original text: Miele
Redaction marker 4: programmer | Original text: freelancer
```

## Using the examples

```
cd Examples/RedactionExample
dotnet user-secrets set "PrivateAi:ApiKey" "<your-api-key>"
```

When you are using a professional api key, change the url in program.cs.
