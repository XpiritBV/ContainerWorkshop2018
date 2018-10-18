# Lab 9 - Prometheus

In this Lab execersise you will install the CoreOS Prometheus Helm chart. This will enable monitoring in your cluster and providing a dashboard for all your services.
If time permits you can add a custom metrics endpoint to your .NET core service and deploy that to the cluster and enable the monitoring of that service using the custom resource types that got created in your AKS cluster by the deployment of the Prometheus package.

Goals for this lab:
- [Add Prometheus monitoring support to your AKS cluster]()
- [Explore the monitoring data using the Grafana dashboard]()
- [Expose a metrics endpoint to your service to provide Prometheus with data]() 
- [explore the custom resource types to monitor your own service]()

## Installing Prometheus

Before we can install Prometheus, we need Helm package management enabled for our Kubernetes Cluster.
So we first install Helm. This is done with the following steps:

Go to the GitHub repository to download the latest zip package that contains the Helm client. You can find the repo here: https://github.com/helm/helm/releases
Unpack the zip file and add the location of Helm.exe to your system path, so you can use this command from any command window you create.

Now we need to install the server part. For this we assume you have a working version of kubectl.exe on your machine and that you can connect to the cluster. To validate this, type the command:
```
kubectl get nodes
```
This should return a set of nodes in your cluster. In our lab setup this should be at least one node.

Now we can install the teller pod, that is required by helm to do our installation of packages.
Type the following command:
```
Helm innit
```
This should return the following:
```
$HELM_HOME has been configured at C:\Users\<youruser>\.helm.
Happy Helming!
```

Now we need to add the CoreOS helm repository to the Helm repositories where it will search for Helm Charts we can install on our cluster.
Adding the repo is done with the following command:

```
helm repo add coreos https://s3-eu-west-1.amazonaws.com/coreos-charts/stable/
```

Now we can find the package by using the command:

```
Helm search coreos/prometheus
```

this reults in the following output if your repo has been correctly registered:
```
NAME                            VERSION DESCRIPTION
coreos/prometheus               0.0.51  Prometheus instance created by the CoreOS Prome...
coreos/prometheus-operator      0.0.29  Provides easy monitoring definitions for Kubern...
```

We need both packages. We first want to install the prometheus-operator package, since that will then take care of the configuration of Prometheus in the cluster. 

Now install the Helm chart by issuing the following command:
```
helm install coreos/prometheus-operator --name prometheus-operator --namespace monitoring --set rbacEnable=false
```
and after this is successful
```
helm install coreos/kube-prometheus --name kube-prometheus --set global.rbacEnable=false --namespace monitoring 
```

Now you should have Prometheus running in your cluster and we can explore the configuration by browsing to the Prometheus build in dashboard

## Browsing the build in dashboard
When you want to look at the dashboard that is provided, we need to be able to browse to the endpoint exposed by the prometheus server. This is located in one of the deployed pods on the cluster. We can use kubectl to do port forwarding for us, so we can browse to the localhost to view it.

To start forwarding the dashboard to your localhost use the following command in a bash shell:
```
kubectl --namespace monitoring port-forward $(kubectl get pod --namespace monitoring -l prometheus=kube-prometheus -l app=prometheus -o template --template "{{(index .items 0).metadata.name}}") 9090:9090
```
Now browse with your browser of choice to the location: http://localhost/targets

this will show you which targets prometheus will scrap for metrics. It also shows a status overview of endpoints if it got data or is dead for a while
The dashboard should show something like this:

<img src="images/prometheus-targets.png" />

## Browsing the Grafana dashboard
With the installation of prometheus, also an instance of the grafana dashboarding is installed. We can find the fact this is true by querying all pods in the `monitor` namespace.

When you execute the following command, you will find there is a pod that contains grafana:
```
kubectl get pods --namespace=monitoring
```
this will result into something similar as the following:
```
NAME                                                   READY   STATUS    RESTARTS   AGE
alertmanager-kube-prometheus-0                         2/2     Running   0          6d
kube-prometheus-exporter-kube-state-7d964949f4-m8kdx   2/2     Running   0          6d
kube-prometheus-exporter-node-2pb48                    1/1     Running   0          6d
kube-prometheus-exporter-node-7p6ls                    1/1     Running   0          6d
kube-prometheus-exporter-node-t6tpc                    1/1     Running   0          6d
kube-prometheus-grafana-6fb79c6c58-wclfm               2/2     Running   0          6d
prometheus-kube-prometheus-0                           3/3     Running   1          6d
prometheus-operator-76c65c6d89-v8dd2                   1/1     Running   0          6d
```
Here we can see in the example that the pod running grafana is the pod with the name: `kube-prometheus-grafana-6fb79c6c58-wclfm`

This pod will expose the grafana website where we can see the metrics provided by prometheus.

Again for this we use port forwarding to enable us to browse to the website on our localhost. We do this with the following command at a bash command prompt:
```
kubectl --namespace monitoring port-forward $(kubectl get pod --namespace monitoring -l app=kube-prometheus-grafana -o template --template "{{(index .items 0).metadata.name}}") 3000:3000
```
Now browse to http://localhost/3000 and there you will see the home of the dashboards. 
<img src="images/grafana-home-dashboard.png" />

Click on the home button in the left upper corner and then select e.g. the Nodes dashboard.

this will result in a similar dashboard as displayed here:

<img src="images/grafanadashboard-node.png"/>

## Creating your custom .NET core metrics Prometheus endpoint

TODODODODO

## Configuring Prometheus custom resource to pick up new metrics

