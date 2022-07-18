FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal

ARG PROJECT
EXPOSE 8080

# VARS
ENV APP_DIR="/app" \
    APP_USER=app \
    # set environtent for aspnetcore
    ASPNETCORE_URLS=http://+:8080

# create app user and ko
RUN mkdir -p ${APP_DIR} \
  && useradd -s /bin/bash -u 1000 -d ${APP_DIR} ${APP_USER} \
  && chown -R ${APP_USER} ${APP_DIR} \
  && chmod 700 ${APP_DIR}

# copy already builded project
COPY --chown=${APP_USER}:${APP_USER} ./output ${APP_DIR}
WORKDIR ${APP_DIR}

# post install commands
RUN rm -f *.sh *.crt \
  && find /bin /lib /sbin /usr -xdev -name chmod -delete

# Run app as non root user
USER ${APP_USER}

ENTRYPOINT ["dotnet", "OCS.TeamAssistant.Appraiser.Backend.dll"]
