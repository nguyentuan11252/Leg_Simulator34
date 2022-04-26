

#import <AudioToolbox/AudioServices.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>


extern "C"
{

static int feedbackSupportLevel = 0;
static UIImpactFeedbackGenerator *lightVibration = NULL;
static UIImpactFeedbackGenerator *mediumVibration = NULL;
static UIImpactFeedbackGenerator *heavyVibration = NULL;

void vibrate(int level)
{
	if (feedbackSupportLevel == 1)
	{ // iPhone 6S
		switch (level) {
			case 0:
			case 1:
			case 2:
				AudioServicesPlaySystemSound(1519);
				break;

			case 3:
				AudioServicesPlaySystemSound(1520);
				break;

			default:
				break;
		}
	}
	else if (feedbackSupportLevel == 2)
	{ // iPhone 7 and above
		switch (level) {
			case 0:
			case 1:
				[lightVibration impactOccurred];
				break;

			case 2:
				[mediumVibration impactOccurred];
				break;

			case 3:
				[heavyVibration impactOccurred];
				break;

			default:
				break;
		}
	}
}

bool isVibrationCustomizable()
{
	feedbackSupportLevel = [[[UIDevice currentDevice] valueForKey:@"_feedbackSupportLevel"] intValue];
	if (feedbackSupportLevel >= 2)
	{
		lightVibration = [[UIImpactFeedbackGenerator alloc] initWithStyle:(UIImpactFeedbackStyleLight)];
		mediumVibration = [[UIImpactFeedbackGenerator alloc] initWithStyle:(UIImpactFeedbackStyleMedium)];
		heavyVibration = [[UIImpactFeedbackGenerator alloc] initWithStyle:(UIImpactFeedbackStyleHeavy)];
	}
	return feedbackSupportLevel > 0;
}

bool advertiserTrackingEnabled()
{
	if (@available(iOS 14, *))
	{
		ATTrackingManagerAuthorizationStatus status = [ATTrackingManager trackingAuthorizationStatus];
		return status == ATTrackingManagerAuthorizationStatusAuthorized;
	}
	return [[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled] == YES;
}

bool isIos14()
{
	if (@available(iOS 14, *))
	{
		return true;
	}
	return false;
}

static bool s_advertiserTrackingPrompted = false;
bool advertiserTrackingPrompted()
{
	if (@available(iOS 14, *))
	{
		return s_advertiserTrackingPrompted;
	}
	return true;
}

void promptAdvertiserTracking()
{
	if (@available(iOS 14, *))
	{
		s_advertiserTrackingPrompted = false;
		ATTrackingManagerAuthorizationStatus status = [ATTrackingManager trackingAuthorizationStatus];
		switch (status)
		{
			case ATTrackingManagerAuthorizationStatusNotDetermined:
				[ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status)
				{
					s_advertiserTrackingPrompted = true;
				}];
				break;
			case ATTrackingManagerAuthorizationStatusDenied:
			case ATTrackingManagerAuthorizationStatusRestricted:
			case ATTrackingManagerAuthorizationStatusAuthorized:
				s_advertiserTrackingPrompted = true;
				break;
		}
	}
	else
	{
		s_advertiserTrackingPrompted = true;
	}
}
}

static struct AutoInitializer
{
	~AutoInitializer()
	{
		lightVibration = NULL;
		mediumVibration = NULL;
		heavyVibration = NULL;
	}
} autoInitializer;

