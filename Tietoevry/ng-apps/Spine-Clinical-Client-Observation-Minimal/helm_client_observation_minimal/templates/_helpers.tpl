{{/*
Expand the name of the chart.
*/}}
{{- define "helm_client_observation_minimal.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "helm_client_observation_minimal.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "helm_client_observation_minimal.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "helm_client_observation_minimal.labels" -}}
helm.sh/chart: {{ include "helm_client_observation_minimal.chart" . }}
{{ include "helm_client_observation_minimal.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "helm_client_observation_minimal.selectorLabels" -}}
app.kubernetes.io/name: {{ include "helm_client_observation_minimal.name" . }}
app.kubernetes.io/instance: {{ .Chart.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "helm_client_observation_minimal.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "helm_client_observation_minimal.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
