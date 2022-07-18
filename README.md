# OCS.TeamAssistant.Appraiser

OCS.TeamAssistant.Appraiser - бот для оценки задач через планнинг покер.

## Telegram bots

<img src="ocs_teamassistant_appraiser.jpg" width="200"/>

Production - ocs_teamassistant_appraiser_bot

Development - ocs_teamassistant_test_bot

## Развертывание

### Сборка образа

```
dotnet build --configuration Release --output output
docker build --build-arg PROJECT=OCS.TeamAssistant.Appraiser.Backend -t ocs.teamassistant.appraiser:{version} .
docker tag ocs.teamassistant.appraiser:{version} dyatlovhome/ocs.teamassistant.appraiser:{version}
docker push dyatlovhome/ocs.teamassistant.appraiser:{version}
```

### Запуск

```
docker run -d dyatlovhome/ocs.teamassistant.appraiser:{version} -p 80:8080 &
```