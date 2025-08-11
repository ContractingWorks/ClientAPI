# Webhook Support

## Overview

ClientAPI provides webhook support to notify external systems about events occurring within the application. Webhooks are triggered by upsert operations on configured entities, sending messages containing the same data as returned in the corresponding upsert API responses.

## Administration

Webhook administration involves managing webhook subscriptions via the `Webhooks` endpoint. The following API endpoints are available:

*   `GET v3/client/{clientid}/webhooks/webhooks`: Returns metadata about available Webhooks. This endpoint provides a list of available webhook event types that you can subscribe to.
*   `GET v3/client/{clientid}/webhooks/subscriptions?integratorIdText={integratorIdText}`: Returns the webhook subscriptions for a given integrator.
*   `POST v3/client/{clientid}/webhooks/subscribe?integratorIdText={integratorIdText}`: Registers or updates webhook subscriptions.
*   `DELETE v3/client/{clientid}/webhooks/delete?webhookSubscriptionId={webhookSubscriptionId}&integratorIdText={integratorIdText}`: Deletes a webhook subscription. If `webhookSubscriptionId` is not provided, all soft-deleted subscriptions for the integrator are permanently removed.

The `integratorIdText` parameter identifies the integrator. The integrator ID is associated with users as a claim, set up by CW support. This allows separation of integrators, so that an admin user can administer webhooks for an integration service user without interacting with webhook definitions belonging to a different integrator. 

For users with an integrator ID already registered on their account, the `integratorIdText` parameter is optional. For ClientAdmin users without a registered integrator ID, they must provide a valid `integratorIdText` in the query string.

### Subscription Parameters

When creating or updating subscriptions via the `POST /subscribe` endpoint, include these parameters:

*   `WebhookSubscriptionId`: The ID of the webhook subscription (required for updates, must not be provided for creation).
*   `Enabled`: Whether the subscription is enabled.
*   `WebhookId`: The ID of the webhook event to subscribe to. You can obtain valid `WebhookId` values by calling the `GET v3/client/{clientid}/webhooks/webhooks` endpoint.
*   `SharedSecret`: A shared secret used to sign the webhook payload (optional). If provided, webhook requests will include an `x-hub-signature-256` header.
*   `Description`: A description of the subscription.
*   `WebhookUrl`: The URL to which webhooks will be sent. Must be an HTTPS URL and cannot point to localhost, private IP addresses, or local file systems.
*   `HttpAuthHeader`: An optional HTTP Authorization header value to include with webhook requests.
*   `Sys_Deactivated`: Indicates if the subscription is deactivated.

## Webhook Delivery

### HTTP Method and Headers

Webhooks are delivered via **HTTP POST** requests to the configured `WebhookUrl`. Each request includes these headers:

*   `Content-Type`: `application/json`
*   `x-hub-signature-256`: (Optional) Present when a `SharedSecret` is configured. Contains the HMACSHA256 hash of the payload, prefixed with `sha256=`.
*   `Authorization`: (Optional) Present when an `HttpAuthHeader` is configured. Contains the value specified in `HttpAuthHeader`.

### Message Format

The JSON payload structure:

```json
{
  "Header": {
    "WebhookId": "string",
    "ClientId": "string",
    "Alg": "HS256" or "None"
  },
  "Data": [
    {
      // Event data - same structure as API responses for corresponding operations
    }
  ]
}
```

**Header Fields:**
*   `WebhookId`: The ID of the webhook event type
*   `ClientId`: The ID of the client (tenant)
*   `Alg`: Signature algorithm - "HS256" when `SharedSecret` is configured, otherwise "None"

**Data Field:**
*   Contains an array of data objects with the same structure as returned by the corresponding API operations
*   Typically includes entity IDs and current state information

### Security

When a `SharedSecret` is configured:
- The `x-hub-signature-256` header contains: `sha256=` + HMACSHA256 hash of the entire JSON payload
- The hash is calculated using the `SharedSecret` as the HMAC key
- This follows the same pattern used by GitHub and Facebook webhooks

### URL Validation

Webhook URLs are validated to prevent security issues:
- Must use HTTPS protocol
- Cannot be localhost or loopback addresses
- Cannot point to private IP address ranges (10.x.x.x, 192.168.x.x, 172.16-31.x.x)
- Cannot point to local file systems

## Events

Available webhook events and their corresponding `WebhookId` values can be retrieved via the `GET v3/client/{clientid}/webhooks/webhooks` endpoint. Only enabled subscriptions will receive webhook deliveries.

## Delivery Guarantees

- **No retries**: Failed webhook deliveries are not retried
- **No guaranteed delivery**: Webhooks are best-effort delivery only
- Failed webhook deliveries do not affect the original API operation
- Only enabled subscriptions receive webhook deliveries
- Only subscriptions with `Enabled = true` receive webhook deliveries
- Webhook URLs are re-validated at delivery time and invalid URLs are skipped

