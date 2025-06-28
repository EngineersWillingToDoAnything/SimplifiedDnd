enum LogStyle {
  Reset = '\x1b[0m',
  Bright = '\x1b[1m',
  Dim = '\x1b[2m',
  Underscore = '\x1b[4m',
  Blink = '\x1b[5m',
  Reverse = '\x1b[7m',
  Hidden = '\x1b[8m',
}

enum LogColor {
  Black = '\x1b[30m',
  Red = '\x1b[31m',
  Green = '\x1b[32m',
  Yellow = '\x1b[33m',
  Blue = '\x1b[34m',
  Magenta = '\x1b[35m',
  Cyan = '\x1b[36m',
  White = '\x1b[37m',
}

export default class Logger {
  constructor(private readonly context: string | undefined = undefined) {}

  logInfo(data: string): void {
    return this.log(data, LogColor.Cyan);
  }

  logSuccess(data: string): void {
    return this.log(data, LogColor.Green);
  }

  logWarning(data: string): void {
    return this.log(data, LogColor.Yellow);
  }

  logError(data: string): void {
    return this.log(data, LogColor.Red);
  }

  logDivider(data: string): void {
    const DIVIDER_SIZE = 30;
    const paddingAmount = DIVIDER_SIZE - data.length;
    let divider = this.toStyle(LogStyle.Bright, data);
    if (paddingAmount > 0) {
      const DIVIDER_CHARACTER = '-';
      const padding = DIVIDER_CHARACTER.repeat(paddingAmount / 2 - 1);
      divider = `${padding} ${divider} ${padding}`;
    }
    console.info(`\n${divider}`);
  }

  private log(data: string, color: LogColor): void {
    let title = `${this.toColor(color, this.parseColorToTitle(color))}:`;
    if (this.context !== undefined) {
      title += ` (${this.context})`;
    }
    console.log(`${title}\n  ${data}`);
  }

  private parseColorToTitle(color: LogColor): string {
    switch (color) {
      case LogColor.Red:
        return 'error';
      case LogColor.Green:
        return 'success';
      case LogColor.Yellow:
        return 'warning';
      case LogColor.Cyan:
        return 'info';
      default:
        return '';
    }
  }

  private toColor(color: LogColor, data: string): string {
    return `${color}${data}${LogStyle.Reset}`;
  }

  private toStyle(style: LogStyle, data: string): string {
    return `${style}${data}${LogStyle.Reset}`;
  }
}
