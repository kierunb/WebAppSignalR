#### Custom Logger

```typescript
import { ILogger, LogLevel } from "@microsoft/signalr";

export class CustomLogger implements ILogger {
    log(logLevel: LogLevel, message: string) {
        // Use `message` and `logLevel` to record the log message to your own system
        console.log(`${logLevel} :: ${message}`);
    }
}
```


#### Retry Policy

```typescript
import { RetryContext } from "@microsoft/signalr";

export default class CustomRetryPolicy implements signalR.IRetryPolicy {
    maxRetryAttempts = 0;

    nextRetryDelayInMilliseconds(retryContext: RetryContext): number {
        console.info(`Retry :: ${retryContext.retryReason}`);
        if (retryContext.previousRetryCount === 10) return null; // stawp!

        var nextRetry = retryContext.previousRetryCount * 1000 || 1000;
        console.log(`Retry in ${nextRetry} milliseconds`);
        return nextRetry;
    }

}
```
