import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-grpc';
import { PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { resourceFromAttributes } from '@opentelemetry/resources';
import { ATTR_SERVICE_NAME } from '@opentelemetry/semantic-conventions';

import Logger from './abstractions/logger';

const otlpEndpoint =
  process.env.OTEL_EXPORTER_OTLP_ENDPOINT ?? 'http://localhost:4317';

const resource = resourceFromAttributes({
  [ATTR_SERVICE_NAME]: 'DiscordBot',
});

const sdk = new NodeSDK({
  instrumentations: [getNodeAutoInstrumentations()],
  metricReader: new PeriodicExportingMetricReader({
    exportIntervalMillis: Number(
      process.env.OTEL_METRIC_EXPORT_INTERVAL ?? '1000',
    ),
    exporter: new OTLPMetricExporter({
      url: otlpEndpoint,
    }),
  }),
  resource,
  traceExporter: new OTLPTraceExporter({
    url: otlpEndpoint,
  }),
});

sdk.start();

const logger = new Logger('Telemetry');
logger.logSuccess('OpenTelemetry configured for Discord Bot');

process.on('SIGTERM', (): void => {
  sdk
    .shutdown()
    .then(() => logger.logSuccess('Tracing terminated'))
    .catch(error => logger.logError(`Error terminating tracing ${error}`))
    .finally(() => process.exit(0));
});
