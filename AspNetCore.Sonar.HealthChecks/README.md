# README

## Unit tests condition

You are required to create user secrets for the unit tests to run. The user secrets should contain the following:

```json
{
  "SonarCloud": {
    "Token": "<your PAT token>",
    "Projects": [ "<project name>", "<project name>" ]
  }
}
```

## Sonar Cloud usage

you will need to have setup an http client and a memory cache in your project