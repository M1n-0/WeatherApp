# Weather App
![logo](/WeatherApp/images/READMElogo.png)

## Table of contents
- [Presentation of the project](#presentation-of-the-project)
- [Prerequisites](#prerequisites)
- [installation and use](#installation-and-use)

## Presentation of the project
This project is a weather application that allows you to know the weather in a city. It is a simple application that uses the OpenWeatherMap API to retrieve the weather data of a city.

## Prerequisites
To use this application, you need to have the .NET SDK installed on your machine. You can download it from the official website: https://dotnet.microsoft.com/download
(We recommend using the latest version of the SDK (9.0 for this project) )

## installation and use
To install the application, you need to download the repository and install the dependencies with the following command:

```bash
git clone https://github.com/M1n-0/WeatherApp
```

Then you need to go inside the folder with the following command:

```bash
cd WeatherApp
```

Do it another time to go inside the project folder:

```bash
cd WeatherApp
```

Before launching the application, you must provide a valid API key in a `config.json` file.

Here is an example of a `config.json` file:

```json
    {
    "ApiKey": "YourApiKey"
    }
```
(put the file in the WeatherApp folder)

For the API Key you need to create an account on the OpenWeatherMap website and get an API key. You can get it from the following link: https://home.openweathermap.org/users/sign_up

And finally, you can run the project with the following command:

```bash
dotnet run
```
